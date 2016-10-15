using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Tests
{
    [TestClass()]
    public class PanKouTests
    {
        [TestMethod()]
        public void LoginTest()
        {
            //Assert.IsTrue(PanKou.Instance.Login("usertest", "123456"));
        }

        [TestMethod()]
        public void CheckTest()
        {
            Assert.IsNotNull(PanKou.Instance.Check());
        }

        [TestMethod()]
        public void TestTest()
        {
            Assert.IsNotNull(PanKou.Instance.Test());
        }
    }
}