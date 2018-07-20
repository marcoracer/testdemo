using Blabla;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blabla.Tests
{
    [TestClass()]
    public class UnitTest1
    {
        [TestMethod()]
        public void SomaSomaTest()
        {
            int y = 10;
            int x = 11;

            Assert.AreEqual(new Calc().Soma(x,y), 21);
        }
    }
}