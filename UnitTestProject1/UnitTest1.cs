using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Simplex;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            VvodPer vz = new VvodPer();
            vz.simplexBol();
            Assert.AreEqual(vz.tableresult[vz.tableresult.GetLength(0) - 1, 0] , - 9.11111111111111);
        }
        [TestMethod]
        public void TestMethod2()
        {
            VvodPer vz = new VvodPer();
            vz.simplexBol();
            Assert.AreEqual(vz.tableresult[vz.tableresult.GetLength(0) - 1, 0] *-1, 9.11111111111111);
        }
        [TestMethod]
        public void TestMethod3()
        {
            VvodPer vz = new VvodPer();
            vz.simplexBol();
            Assert.AreEqual(vz.tableresult[0,0], 0.888888888888889);
        }
        
    }
}
