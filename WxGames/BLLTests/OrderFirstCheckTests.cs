using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Tests
{
    [TestClass()]
    public class OrderFirstCheckTests
    {
        [TestMethod()]
        public void CheckOneTest()
        {
            OrderFirstCheck.Instance.CheckOne("我上分了");
        }
    }
}