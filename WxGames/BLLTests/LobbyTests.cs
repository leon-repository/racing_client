using Microsoft.VisualStudio.TestTools.UnitTesting;
using WxGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL;
using System.Configuration;
using Model;

namespace WxGames.Tests
{
    [TestClass()]
    public class LobbyTests
    {
        [TestMethod()]
        public void NewMethodTest()
        {
            DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());
            List<NowMsg> msgList = data.GetList<NowMsg>(" isdelete=1 and CommandType in ('买名次','冠亚和','名次大小单双龙虎') ", "");
            Lobby.NewMethod(data, msgList, "2328346900");
            //Assert.Fail();
        }
    }
}