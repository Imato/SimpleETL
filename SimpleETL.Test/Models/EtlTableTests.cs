namespace Imato.SimpleETL.Test.Models
{
    public class EtlTableTests
    {
        [Test]
        public void CreateTest()
        {
            IEtlTable table = new EtlTable(20);
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            row["Col100"] = "Test";
            row["Test"] = "Test2";
            row["Date"] = DateTime.Now;

            for (int i = 0; i < 10; i++)
            {
                table.AddRow(row.Copy());
                if (i == 5)
                {
                    table[5]["Col1"] = 101;
                    table[5]["Test"] = "3";
                    table[5]["Test5"] = 5;
                }
            }

            Assert.That(20, Is.EqualTo(table.BufferSize));
            Assert.That(10, Is.EqualTo(table.RowCount));
            Assert.That(10, Is.EqualTo(table.Rows.Count()));
            Assert.That(100, Is.EqualTo(table[9][0]));
            Assert.That("Test2", Is.EqualTo(table[9][2]));
            Assert.That(101, Is.EqualTo(table[5][0]));
            Assert.That("3", Is.EqualTo(table[5][2]));
            Assert.That(5, Is.EqualTo(table[5]["Test5"]));
            Assert.That(5, Is.EqualTo(table[5][4]));
        }

        [Test]
        public void ClearTest()
        {
            IEtlTable table = new EtlTable(20);
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            row["Col100"] = "Test";
            row["Test"] = "Test2";
            row["Date"] = DateTime.Now;

            for (int i = 0; i < 10; i++)
            {
                table.AddRow(row.Copy());
            }

            table.Clear();
            Assert.That(0, Is.EqualTo(table.RowCount));
            Assert.Throws<IndexOutOfRangeException>(() => { var t = table[0]; });
        }

        [Test]
        public void BufferSizeTest()
        {
            IEtlTable table = new EtlTable(2);
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            row["Col100"] = "Test";
            row["Test"] = "Test2";
            row["Date"] = DateTime.Now;

            table.AddRow(row);
            table.AddRow(row);
            Assert.Throws<Exception>(() => table.AddRow(row));
        }
    }
}