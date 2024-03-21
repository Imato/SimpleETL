namespace Imato.SimpleETL.Test.Models
{
    public class EtlDataFlowTests
    {
        [Test]
        public void CreateTest()
        {
            IEtlDataFlow flow = new EtlDataFlow();
            Assert.That(flow.ColumnsCount, Is.EqualTo(0));
            Assert.That(flow.Columns.Count(), Is.EqualTo(0));
            Assert.That(flow.HasColumn("Test"), Is.False);
            Assert.That(flow.GetColumn("Test"), Is.Null);
            Assert.That(flow.GetColumn(10), Is.Null);
        }

        [Test]
        public void AddTests()
        {
            IEtlDataFlow flow = new EtlDataFlow();
            IEtlRow row = new EtlRow(flow);
            row["String"] = "TestValue";
            row["Date"] = DateTime.Now;
            row["Int"] = 100;

            Assert.That(flow.ColumnsCount, Is.EqualTo(3));
            Assert.That(flow.Columns.Count(), Is.EqualTo(3));
            Assert.That(flow.HasColumn("String"), Is.True);
            Assert.That(flow.GetColumn("Date").Id, Is.EqualTo(1));
            Assert.That(flow.GetColumn(2).Name, Is.EqualTo("Int"));

            Assert.That(flow.HasColumn("Test"), Is.False);
            Assert.That(flow.GetColumn("Test"), Is.Null);
            Assert.That(flow.GetColumn(10), Is.Null);
        }

        [Test]
        public void NewRowTests()
        {
            IEtlDataFlow flow = new EtlDataFlow();
            IEtlRow row = new EtlRow(flow);
            row["String"] = "TestValue";
            row["Date"] = DateTime.Now;
            row["Int"] = 100;

            var row2 = row.Copy();
            Assert.That(row.ColumnsCount, Is.EqualTo(row2.ColumnsCount));
            Assert.That(flow.ColumnsCount, Is.EqualTo(row2.ColumnsCount));
            Assert.That(flow, Is.EqualTo(row.Flow));
            Assert.That(flow, Is.EqualTo(row2.Flow));
        }

        [Test]
        public void IncreaseTest()
        {
            IEtlDataFlow flow = new EtlDataFlow();

            for (int i = 1; i <= 100; i++)
            {
                flow.AddColumn(i.ToString(), i.GetType());
            }

            Assert.That(100, Is.EqualTo(flow.ColumnsCount));
            Assert.That("100", Is.EqualTo(flow.GetColumn(99).Name));
            Assert.That(99, Is.EqualTo(flow.GetColumn("100").Id));
        }

        [Test]
        public void OrderTest()
        {
            IEtlDataFlow flow = new EtlDataFlow();
            flow.AddColumn("1", "1".GetType());
            flow.AddColumn("5", "5".GetType());
            flow.AddColumn("100", "100".GetType());
            flow.AddColumn("3", "3".GetType());
            flow.AddColumn("99", "99".GetType());

            Assert.That(0, Is.EqualTo(flow.GetColumn("1").Id));
            Assert.That(1, Is.EqualTo(flow.GetColumn("5").Id));
            Assert.That(2, Is.EqualTo(flow.GetColumn("100").Id));
            Assert.That(3, Is.EqualTo(flow.GetColumn("3").Id));
            Assert.That(4, Is.EqualTo(flow.GetColumn("99").Id));
        }
    }
}