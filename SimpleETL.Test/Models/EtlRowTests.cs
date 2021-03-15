using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleETL.Test.Models
{
    [TestClass]
    public class EtlRowTests
    {
        [TestMethod]
        public void CreateTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            row["Col100"] = "Test";
            row["Test"] = "Test2";
            row["Date"] = DateTime.Now;

            Assert.AreEqual(4, row.ColumnsCount);
            Assert.AreEqual("Test", row["Col100"]);
            Assert.IsNull(row["Col101"]);
            Assert.AreEqual(100, row[0]);
        }

        [TestMethod]
        public void CheckTypeTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col100"] = 100;
            Assert.AreEqual(100, row["Col100"]);
            row["Col1"] = "Test";
            row["Object"] = null;
            Assert.ThrowsException<TypeAccessException>(() => row["Col100"] = "Test");
        }

        [TestMethod]
        public void GetColumnTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            row["Col100"] = "Test";
            row["Test"] = "Test2";
            row["Date"] = DateTime.Now;

            Assert.AreEqual(4, row.ColumnsCount);
            Assert.AreEqual("Test", row["Col100"]);
            Assert.IsNull(row["Col101"]);
            Assert.AreEqual(100, row[0]);

            Assert.AreEqual("Col1", row.ColumnName(0));
            Assert.AreEqual("Date", row.ColumnName(3));

            Assert.AreEqual(1, row.ColumnId("Col100"));
            Assert.AreEqual(2, row.ColumnId("Test"));

            Assert.ThrowsException<IndexOutOfRangeException>(() => row.ColumnName(15));
            Assert.ThrowsException<IndexOutOfRangeException>(() => row.ColumnId("Test3"));
        }

        [TestMethod]
        public void ClearTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            Assert.AreEqual(100, row["Col1"]);
            row.Clear();
            Assert.IsNull(row["Col1"]);
        }

        [TestMethod]
        public void CopyTest()
        {
            IEtlRow row = new EtlRow(new EtlDataFlow());
            row["Col1"] = 100;
            Assert.AreEqual(100, row["Col1"]);

            IEtlRow row2 = row.Copy();
            Assert.AreEqual(row.Flow, row2.Flow);
            Assert.AreEqual(row.ColumnsCount, row2.ColumnsCount);
            Assert.AreEqual(100, row2["Col1"]);
        }
    }
}
