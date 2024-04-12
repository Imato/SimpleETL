using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Threading;

namespace Imato.SimpleETL
{
    public class KafkaSource : DataSource
    {
        private const int TIME_OUT = 10000;

        private readonly int _maxReadRows, _maxRunTime, _batchSize;
        private readonly string _brokersList, _topicsList;

        private Dictionary<TopicPartition, Offset> _topics;

        /// <summary>
        /// Prepare Kafka source
        /// </summary>
        /// <param name="brokersList">Example: 192.168.48.84:9092;192.168.48.86:9092</param>
        /// <param name="topicsList">Example: "[{""topic"": ""infrastructureSystemMessages"", ""partition"": 0, ""offset"": 131311},...]"</param>
        /// <param name="maxReadRows">Max read rows per one load</param>
        /// <param name="maxRunTime">Max load time in milliseconds</param>
        public KafkaSource(string brokersList, string topicsList, int maxReadRows = 0, int maxRunTime = 0, int batchSize = 1000)
        {
            _maxReadRows = maxReadRows;
            _maxRunTime = maxRunTime;
            _brokersList = brokersList;
            _topicsList = topicsList;
            _batchSize = batchSize;
        }

        private IConsumer<Ignore, string> GetConsumer()
        {
            var connect = Connect<Ignore>(_brokersList);
            var consumer = connect.Consumer;
            var admin = connect.Admin;

            // Get assignments list and assign

            Dictionary<TopicPartition, Offset> topics = GetTopics(_topicsList);
            var topicList = new HashSet<string>();

            foreach (var t in topics.Keys)
            {
                if (!topicList.Contains(t.Topic))
                    topicList.Add(t.Topic);
            }

            Debug($"Subscribe for topics {string.Join(',', topicList.ToList())}");

            consumer.Subscribe(topicList);

            /*
            // Uncomment for using seek back

            var metadataTopics = admin.GetMetadata(TimeSpan.FromMilliseconds(TIME_OUT))
                .Topics
                .Where(t => topicList.Contains(t.Topic));

            var assignments = new List<TopicPartitionOffset>();

            foreach (var t in metadataTopics)
            {
                foreach (var p in t.Partitions)
                {
                    var tp = new TopicPartition(t.Topic, p.PartitionId);
                    if (topics.ContainsKey(tp))
                        assignments.Add(new TopicPartitionOffset(tp, topics[tp].Value));
                    else
                    {
                        assignments.Add(new TopicPartitionOffset(tp, Offset.Beginning.Value));
                        topics.Add(tp, 0);
                    }
                }
            }

            Debug($"Kafka assignments {string.Join(',', assignments.ToList())}");

            consumer.Assign(assignments);

            foreach (var a in assignments)
            {
                Debug($"Seek partition to {a}");
                consumer.Seek(a);
            }

            */

            _topics = topics;
            return consumer;
        }

        public string Topics
        {
            get
            {
                var topic = @"""topic"": ""{0}"", ""partition"": {1}, ""offset"": {2}";
                var sb = new StringBuilder("[");

                foreach (var t in _topics)
                {
                    sb.Append("{");
                    sb.Append(string.Format(topic, t.Key.Topic, t.Key.Partition.Value, t.Value.Value));
                    sb.Append("},");
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
                return sb.ToString();
            }
        }

        private (IConsumer<K, string> Consumer, IAdminClient Admin) Connect<K>(string brokersList)
        {
            var brokers = brokersList.Split(";").OrderBy(x => Guid.NewGuid());

            foreach (var b in brokers)
            {
                try
                {
                    Debug($"Try to connect Kafka server {b}");

                    var config = new ConsumerConfig
                    {
                        BootstrapServers = b,
                        GroupId = "dashboard-consumer",
                        EnableAutoCommit = true,
                        StatisticsIntervalMs = 10000,
                        SessionTimeoutMs = 10000
                    };

                    var cBulder = new ConsumerBuilder<K, string>(config);
                    var aBuilder = new AdminClientBuilder(config);

                    Debug("Connected");

                    return (cBulder.Build(), aBuilder.Build());
                }
                catch { }
            }

            throw new ApplicationException($"All brokers {brokersList} are down!");
        }

        private Dictionary<TopicPartition, Offset> GetTopics(string topics)
        {
            var result = new Dictionary<TopicPartition, Offset>();

            if (!string.IsNullOrEmpty(topics))
            {
                foreach (var a in JArray.Parse(topics))
                {
                    var topic = new TopicPartition(a["topic"].Value<string>(), a["partition"].Value<int>());
                    var offsetValue = a["offset"].Value<long>();
                    var offset = offsetValue <= 0 ? Offset.Beginning : new Offset(offsetValue);

                    if (!result.ContainsKey(topic))
                        result.Add(topic, offset);
                }
            }

            return result;
        }

        public override IEnumerable<IEtlRow> GetData(CancellationToken token = default)
        {
            var rows = 0;
            var cancelled = false;
            LastDate = DateTime.Now;

            using (var consumer = GetConsumer())
            {
                while (!cancelled && !token.IsCancellationRequested)
                {
                    var msg = consumer.Consume(TIME_OUT);

                    if (msg != null)
                    {
                        var row = CreateRow();
                        row["Message"] = msg.Message.Value;
                        row["Offset"] = msg.Offset;
                        row["Partition"] = msg.Partition;
                        row["Topic"] = msg.Topic;
                        rows++;
                        _topics[msg.TopicPartition] = msg.Offset;
                        yield return row;
                    }

                    if (msg == null)
                    {
                        Debug("Wait new message");
                        Thread.Sleep(1000);
                    }

                    cancelled = Canceled(LastDate, rows);

                    if (rows % _batchSize == 0)
                    {
                        Debug($"Proceed {rows} rows");
                    }
                }

                Debug($"Total rows {rows}");
                Debug("Done");
                consumer.Unsubscribe();
            }
        }

        private bool Canceled(DateTime startDate, int rows)
        {
            if (_maxReadRows == 0 && _maxRunTime == 0)
                return false;

            return ((_maxReadRows > 0 && rows > _maxReadRows)
                    || (_maxRunTime > 0 && (DateTime.Now - startDate).TotalMilliseconds > _maxRunTime));
        }
    }
}