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

        [TestMethod()]
        public void UpScoreTest()
        {
            //Assert.IsTrue(ScoreManager.Instance.UpScore("2328346900", 500));

            //Assert.IsTrue(ScoreManager.Instance.UpScore("437364022", 500));
            Assert.IsTrue(ScoreManager.Instance.UpScore("1470491868", 2000));
            

        }
    }
}