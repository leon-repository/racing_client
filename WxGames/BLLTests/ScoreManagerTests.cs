using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Tests
{
    [TestClass()]
    public class ScoreManagerTests
    {
        [TestMethod()]
        public void GetUpDowModelTest()
        {
            ScoreManager.Instance.GetUpDowModel();

        }
    }
}