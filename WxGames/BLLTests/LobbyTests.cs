using Microsoft.VisualStudio.TestTools.UnitTesting;
using WxGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL;
using System.Configuration;
using Model;
using WxGames.Body;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        [TestMethod()]
        public void ToJson()
        {
            List<KaiJianBody> list = new List<KaiJianBody>();
            list.Add(new KaiJianBody() { members = new Members(), memberStake = new MemberStake() });
            list.Add(new KaiJianBody() { members = new Members(), memberStake = new MemberStake() });

            string json = JsonConvert.SerializeObject(list);

            List<KaiJianBody> listJson = JsonConvert.DeserializeObject<List<KaiJianBody>>(json);
        }

        [TestMethod()]
        public void NewMethod1Test()
        {
            StakeVoMax vo = new StakeVoMax();
            vo.wechatSn = "";
            StakeVo vo2 = new StakeVo();
            vo.stakeVo = vo2;
            vo2.appointStakeList = new List<AppointStake>();
            vo2.appointStakeList.Add(new AppointStake() { carNum = 1 });
            vo2.appointStakeList.Add(new AppointStake() { carNum = 2 });
            vo2.appointStakeList.Add(new AppointStake() { carNum = 3 });
            vo2.appointStakeList.Add(new AppointStake() { carNum = 4 });
            vo2.appointStakeList.Add(new AppointStake() { carNum = 5 });
            vo2.appointStakeList.Add(new AppointStake() { carNum = 6 });
            vo2.appointStakeList.Add(new AppointStake() { carNum = 7 });
            vo2.appointStakeList.Add(new AppointStake() { carNum = 8 });
            vo2.appointStakeList.Add(new AppointStake() { carNum = 9 });
            vo2.appointStakeList.Add(new AppointStake() { carNum = 10 });
            vo2.commonStake = new CommonStake();
            vo2.rankingStakeList = new List<RankingStake>();
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 1 });
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 2 });
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 3 });
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 4 });
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 5 });
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 6 });
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 7 });
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 8 });
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 9 });
            vo2.rankingStakeList.Add(new RankingStake() { rankingNum = 10 });

            NowMsg now = new NowMsg();
            now.CommandOne = "和";
            now.CommandTwo = "20/";
            now.CommandType = "冠亚和";
            now.Score = "10";

            Lobby.NewMethod1(vo2, now);
        }

        [TestMethod()]
        public void JObject2Object()
        {
            string json = "{\"result\" : \"SUCCESS\",\"data\" : [ {\"members\" : {\"id\" : 3,\"userId\" : 5,\"wechatClientId\" : null,\"wechatSn\" : \"2039521281\",\"nickName\" : \"当机游戏\",\"points\" : 1090.00},\"memberStake\" : {\"id\" : 24,\"membersId\" : 3,\"racingNum\" : \"201610190262\",\"totalStakeAmount\" : 10.00,\"totalIncomeAmount\" : 0.00,\"totalDeficitAmount\" : -10.00,\"totalStakeCount\" : 1,\"isComplateStatistics\" : true,\"createTime\" : 1476901552000}} ],\"message\" : null,\"page\" : 0,\"pageSize\" : 0,\"totalPage\" : 0,\"count\" : 0}";

            JObject objc= JsonConvert.DeserializeObject(json) as JObject;

            string data=objc["data"].ToString();

            List<KaiJianBody> listBody = JsonConvert.DeserializeObject<List<KaiJianBody>>(data);

        }
    }
}