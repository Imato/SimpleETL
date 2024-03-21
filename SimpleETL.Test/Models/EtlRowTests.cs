namespace Imato.SimpleETL.Test.Models
{
    public class EtlRowTests
    {
        [Test]
        public void CreateTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            row["Col100"] = "Test";
            row["Test"] = "Test2";
            row["Date"] = DateTime.Now;

            Assert.That(4, Is.EqualTo(row.ColumnsCount));
            Assert.That("Test", Is.EqualTo(row["Col100"]));
            Assert.That(row["Col101"], Is.Null);
            Assert.That(100, Is.EqualTo(row[0]));
        }

        [Test]
        public void CheckTypeTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col100"] = 100;
            Assert.That(100, Is.EqualTo(row["Col100"]));
            row["Col1"] = "Test";
            row["Object"] = null;
            Assert.Throws<TypeAccessException>(() => row["Col100"] = "Test");
        }

        [Test]
        public void GetColumnTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            row["Col100"] = "Test";
            row["Test"] = "Test2";
            row["Date"] = DateTime.Now;

            Assert.That(4, Is.EqualTo(row.ColumnsCount));
            Assert.That("Test", Is.EqualTo(row["Col100"]));
            Assert.That(row["Col101"], Is.Null);
            Assert.That(100, Is.EqualTo(row[0]));

            Assert.That("Col1", Is.EqualTo(row.ColumnName(0)));
            Assert.That("Date", Is.EqualTo(row.ColumnName(3)));

            Assert.That(1, Is.EqualTo(row.ColumnId("Col100")));
            Assert.That(2, Is.EqualTo(row.ColumnId("Test")));

            Assert.Throws<IndexOutOfRangeException>(() => row.ColumnName(15));
            Assert.Throws<IndexOutOfRangeException>(() => row.ColumnId("Test3"));
        }

        [Test]
        public void ClearTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            Assert.That(100, Is.EqualTo(row["Col1"]));
            row.Clear();
            Assert.That(row["Col1"], Is.Null);
        }

        [Test]
        public void CopyTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            Assert.That(100, Is.EqualTo(row["Col1"]));

            IEtlRow row2 = row.Copy();
            Assert.That(row.Flow, Is.EqualTo(row2.Flow));
            Assert.That(row.ColumnsCount, Is.EqualTo(row2.ColumnsCount));
            Assert.That(100, Is.EqualTo(row2["Col1"]));
        }
    }
}