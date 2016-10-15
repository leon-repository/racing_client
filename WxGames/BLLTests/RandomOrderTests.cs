using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Tests
{
    [TestClass()]
    public class RandomOrderTests
    {
        [TestMethod()]
        public void GetRandomOrderTest()
        {
            string command=RandomOrder.Instance.GetRandomOrder();
            
        }
    }
}