using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace SimpleETL.Test.Models
{
    [TestClass]
    public class EtlDataFlowTests
    {
        [TestMethod]
        public void CreateTest()
        {
            IEtlDataFlow flow = new EtlDataFlow();
            Assert.IsTrue(flow.ColumnsCount == 0);
            Assert.IsTrue(flow.Columns.Count() == 0);
            Assert.IsFalse(flow.HasColumn("Test"));
            Assert.IsTrue(flow.GetColumn("Test") == null);
            Assert.IsTrue(flow.GetColumn(10) == null);
        }

        [TestMethod]
        public void AddTests()
        {
            IEtlDataFlow flow = new EtlDataFlow();
            IEtlRow row = new EtlRow(flow);
            row["String"] = "TestValue";
            row["Date"] = DateTime.Now;
            row["Int"] = 100;

            Assert.IsTrue(flow.ColumnsCount == 3);
            Assert.IsTrue(flow.Columns.Count() == 3);
            Assert.IsTrue(flow.HasColumn("String"));
            Assert.AreEqual(1, flow.GetColumn("Date").Id);
            Assert.AreEqual("Int", flow.GetColumn(2).Name);

            Assert.IsFalse(flow.HasColumn("Test"));
            Assert.IsTrue(flow.GetColumn("Test") == null);
            Assert.IsTrue(flow.GetColumn(10) == null);
        }

        [TestMethod]
        public void NewRowTests()
        {
            IEtlDataFlow flow = new EtlDataFlow();
            IEtlRow row = new EtlRow(flow);
            row["String"] = "TestValue";
            row["Date"] = DateTime.Now;
            row["Int"] = 100;

            var row2 = row.Copy();
            Assert.AreEqual(row.ColumnsCount, row2.ColumnsCount);
            Assert.AreEqual(flow.ColumnsCount, row2.ColumnsCount);
            Assert.AreEqual(flow, row.Flow);
            Assert.AreEqual(flow, row2.Flow);
        }

        [TestMethod]
        public void IncreaseTest()
        {
            IEtlDataFlow flow = new EtlDataFlow();

            for (int i = 1; i <= 100; i++)
            {
                flow.AddColumn(i.ToString(), i.GetType());
            }

            Assert.AreEqual(100, flow.ColumnsCount);
            Assert.AreEqual("100", flow.GetColumn(99).Name);
            Assert.AreEqual(99, flow.GetColumn("100").Id);
        }

        [TestMethod]
        public void OrderTest()
        {
            IEtlDataFlow flow = new EtlDataFlow();
            flow.AddColumn("1", "1".GetType());
            flow.AddColumn("5", "5".GetType());
            flow.AddColumn("100", "100".GetType());
            flow.AddColumn("3", "3".GetType());
            flow.AddColumn("99", "99".GetType());

            Assert.AreEqual(0, flow.GetColumn("1").Id);
            Assert.AreEqual(1, flow.GetColumn("5").Id);
            Assert.AreEqual(2, flow.GetColumn("100").Id);
            Assert.AreEqual(3, flow.GetColumn("3").Id);
            Assert.AreEqual(4, flow.GetColumn("99").Id);
        }

    }
}
