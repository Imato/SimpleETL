using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;


namespace SimpleETL.Test.Models
{
    [TestClass]
    public class EtlTableTests
    {
        [TestMethod]
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

            Assert.AreEqual(20, table.BufferSize);
            Assert.AreEqual(10, table.RowCount);
            Assert.AreEqual(10, table.Rows.Count());
            Assert.AreEqual(100, table[9][0]);
            Assert.AreEqual("Test2", table[9][2]);
            Assert.AreEqual(101, table[5][0]);
            Assert.AreEqual("3", table[5][2]);
            Assert.AreEqual(5, table[5]["Test5"]);
            Assert.AreEqual(5, table[5][4]);

        }

        [TestMethod]
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
            Assert.AreEqual(0, table.RowCount);
            Assert.ThrowsException<IndexOutOfRangeException>(() => table[0]);

        }

        [TestMethod]
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
            Assert.ThrowsException<Exception>(() => table.AddRow(row));

        }
    }
}
