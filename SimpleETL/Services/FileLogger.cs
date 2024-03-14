using System;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using SimpleETL.Models;

namespace SimpleETL
{
    public class FileLogger : BaseLoger
    {
        private static FileStream file;
        private static StreamWriter writer;
        private static object lockObject = new object();
        private static ConcurrentQueue<string> buffer;
        private const int bufferSize = 100;

        public FileLogger(IConfigurationService configuration) : base(configuration)
        {
            if (!string.IsNullOrEmpty(_configuration.LogFileFolder))
                lock (lockObject)
                    if (writer == null)
                    {
                        if (!Directory.Exists(_configuration.LogFileFolder))
                            Directory.CreateDirectory(_configuration.LogFileFolder);

                        if(_configuration.LogFilesHistoryDays > 0)
                        {
                            var minDate = DateTime.Now.AddDays(-1 * _configuration.LogFilesHistoryDays);
                            foreach (var file in Directory.GetFiles(_configuration.LogFileFolder, "*-*-*.log", SearchOption.TopDirectoryOnly))
                            {
                                if (File.GetCreationTime(file) < minDate)
                                    File.Delete(file);
                            }
                        }                        

                        buffer = new ConcurrentQueue<string>();

                        var fileName = $"{_configuration.LogFileFolder}/{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.log";
                        file = File.Open(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                        writer = new StreamWriter(file, Encoding.UTF8);
                    }
        }

        public override void WriteLog(object source, string message, LogLevel level)
        {
            if (writer == null)
                return;

            try
            {
                var text = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss.mmm} {level} {source}: {message}";
                writer.WriteLine(text);
            }

            catch
            {
               
            }
            
            //buffer.Enqueue(text);
            //WriteBuffer();
        }

        private void WriteBuffer(bool writeAll = false)
        {
            if (buffer.Count < bufferSize && !writeAll)
                return;

            if (writer == null)
                return;

            while (buffer.Count > 0)
            {
                buffer.TryDequeue(out string text);
                writer.WriteLine(text);
            }                          
        }

        public override void Dispose()
        {
            if (writer == null)
                return;

            WriteBuffer(true);
            writer.Dispose();
            file.Dispose();
        }
    }
}
