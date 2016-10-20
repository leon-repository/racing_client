using BLL;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WxGames.Body;

namespace WxGames
{
    public class Lobby
    {
        private Lobby() { }

        public static readonly Lobby Instanc = new Lobby();

        public void Start()
        {
            Log.WriteLogByDate("开始获取获取加密");
            DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());
            //获取开奖信息，并将开奖信息保存到数据库
            string urlConfiger = "/user/client/stake/configer";
            string auth = PanKou.Instance.GetSha1("", urlConfiger);
            Log.WriteLogByDate("结束获取获取加密");

            Log.WriteLogByDate("开始获取开奖信息");
            string json = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger, auth, PanKou.accessKey);
            Log.WriteLogByDate("结束获取开奖信息");
            //Log.WriteLogByDate("获取开奖信息：" + json);
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            JObject configHelper = JsonConvert.DeserializeObject(json) as JObject;
            if (configHelper == null)
            {
                return;
            }
            if (configHelper["result"].ToString() != "SUCCESS")
            {
                return;
            }
            string gameId = configHelper["data"]["racingNum"].ToString();
            if (string.IsNullOrEmpty(gameId))
            {
                Log.WriteLogByDate("发生异常：当前期号为空，上期期号为" + configHelper["data"]["preRacingNum"].ToString());
                return;
            }

            string nextStartTime = configHelper["data"]["startRacingTime"].ToString();
            string stage = configHelper["data"]["stage"].ToString();//stage=1,押注阶段；stage=2,上报阶段；stage=3,封盘阶段
            if (frmMainForm.IsContinue)
            {
                frmMainForm.Perioid = gameId;
                List<KeyValuePair<string, object>> pkrace = new List<KeyValuePair<string, object>>();
                pkrace.Add(new KeyValuePair<string, object>("GameId", gameId));
                Game game = data.First<Game>(pkrace, "");
                if (game == null)//信息不存在
                {
                    game = new Game();
                    game.Uuid = Guid.NewGuid().ToString();
                    game.GameId = gameId;
                    game.EndTime = (DateTime.Now.DateTimeToUnixTimestamp() + Convert.ToInt64(nextStartTime) / 1000).ToString();
                    game.StartTime = (Convert.ToInt64(game.EndTime) - 300).ToString();
                    game.IsSucc = "0";
                    Game gameInit2 = data.First<Game>(pkrace, "");
                    if (gameInit2 == null)
                    {
                        Log.WriteLogByDate("开始插入数据库消息");
                        data.Insert<Game>(game, "");
                        //把以前的消息全修改为发送
                        data.ExecuteSql(" update GameMsg set issend='1' ");
                        List<Config> list = frmMainForm.Configs;
                        //向每期消息表插入信息
                        if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK3").Value))
                        {
                            //280秒
                            GameMsg gameMsg1 = new GameMsg();
                            gameMsg1.Uuid = Guid.NewGuid().ToString();
                            gameMsg1.GameId = game.GameId;
                            gameMsg1.SendTime = game.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "PTTIME3").Value.ToInt();
                            gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT3").Value;
                            gameMsg1.IsSend = "0";
                            data.Insert<GameMsg>(gameMsg1, "");
                        }

                        //110秒
                        if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK2").Value))
                        {
                            GameMsg gameMsg1 = new GameMsg();
                            gameMsg1.Uuid = Guid.NewGuid().ToString();
                            gameMsg1.GameId = game.GameId;
                            gameMsg1.SendTime = game.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "PTTIME2").Value.ToInt();
                            gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT2").Value;
                            gameMsg1.IsSend = "0";
                            data.Insert<GameMsg>(gameMsg1, "");
                        }

                        //封盘 60秒
                        if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "FPCHK").Value))
                        {
                            GameMsg gameMsg1 = new GameMsg();
                            gameMsg1.Uuid = Guid.NewGuid().ToString();
                            gameMsg1.GameId = game.GameId;
                            gameMsg1.SendTime = game.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "FPTIME").Value.ToInt();
                            gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "FPCONTENT").Value;
                            gameMsg1.IsSend = "0";
                            data.Insert<GameMsg>(gameMsg1, "");
                        }

                        //30秒
                        if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK").Value))
                        {
                            GameMsg gameMsg1 = new GameMsg();
                            gameMsg1.Uuid = Guid.NewGuid().ToString();
                            gameMsg1.GameId = game.GameId;
                            gameMsg1.SendTime = game.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "PTTIME").Value.ToInt();
                            gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT").Value;
                            gameMsg1.IsSend = "0";
                            data.Insert<GameMsg>(gameMsg1, "");
                        }
                        Log.WriteLogByDate("结束插入数据库消息");
                    }
                }
            }

            //获取未发送的消息
            List<GameMsg> listMsg = data.GetList<GameMsg>(" isSend='0' ", "");

            foreach (GameMsg msg in listMsg)
            {
                if (msg.SendTime <= DateTime.Now.DateTimeToUnixTimestamp())
                {
                    data.ExecuteSql(string.Format(" update gamemsg set issend='1' where uuid='{0}' ", msg.Uuid));

                    string content = msg.Content;
                    //查找下注信息
                    if (content.Contains("[下注信息]"))
                    {
                        //生成下注信息
                        try
                        {
                            content = msg.Content.Replace("[下注信息]", "");
                            List<NowMsg> nowMsgList = data.GetList<NowMsg>(string.Format(" CommandType not in ('上下查') and isdelete='2' and period='{0}' ", frmMainForm.Perioid), "");
                            foreach (NowMsg nowMsg in nowMsgList)
                            {
                                //1,处理指令
                                WXMsg model = Msg2WxMsg.Instance.GetMsg2(nowMsg);
                                string msgMessage = model.Msg;
                                content = content + msgMessage;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLogByDate("获取下注信息出错");
                            Log.WriteLog(ex);
                        }
                    }

                    Log.WriteLogByDate("发送开奖结果消息");
                    frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = content, To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);
                    Log.WriteLogByDate("结束开奖结果消息");
                }
            }

            //判断阶段
            //下注信息上传
            if (stage == "2")
            {
                //Log.WriteLogByDate("当前期号：" + frmMainForm.Perioid);
                Log.WriteLogByDate("开始上报");
                frmMainForm.IsContinue = false;
                List<NowMsg> msgList = new List<NowMsg>();
                msgList = data.GetList<NowMsg>(" isdelete=1 and CommandType in ('买名次','冠亚和','名次大小单双龙虎') and period= " + frmMainForm.Perioid, "");
                //下注信息解析成押注信息上传到服务器
                List<string> msgFromIdList = msgList.Select(p => p.MsgFromId).Distinct<string>().ToList();
                foreach (string fromId in msgFromIdList)
                {
                    if (string.IsNullOrEmpty(fromId))
                    {
                        continue;
                    }
                    try
                    {
                        NewMethod(data, msgList, fromId);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLogByDate("上报阶段");
                        Log.WriteLog(ex);
                    }
                }
                Log.WriteLogByDate("结束上报");
                frmMainForm.IsKaiJian = true;
            }
            //查询开奖结果，发送本期开奖结果
            if (frmMainForm.IsKaiJian)
            {
                try
                {
                    List<KeyValuePair<string, object>> queryString = new List<KeyValuePair<string, object>>();
                    queryString.Add(new KeyValuePair<string, object>("racingNum", frmMainForm.Perioid));

                    queryString = queryString.OrderBy(p => p.Key).ToList();

                    string queryValue = "";
                    foreach (KeyValuePair<string, object> item in queryString)
                    {
                        queryValue = queryValue + item.Value.ToString();
                    }

                    string urlConfiger2 = "/user/record/result";
                    string auth2 = PanKou.Instance.GetSha1("", urlConfiger2 + queryValue);

                    string json2 = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger2 + "?racingNum=" + frmMainForm.Perioid, auth2, PanKou.accessKey);

                    if (string.IsNullOrEmpty(json2))
                    {
                        return;
                    }
                    JObject configHelper2 = JsonConvert.DeserializeObject(json2) as JObject;
                    string result = configHelper2["result"].ToString();
                    string message = configHelper2["message"].ToString();
                    if (result == "SUCCESS")
                    {
                        string resultArray = configHelper2["data"]["result"].ToString();
                        resultArray = resultArray.Replace("\r\n", "");
                        string racingNum = configHelper2["data"]["racingNum"].ToString();
                        List<Config> list = frmMainForm.Configs;
                        //发送开奖结果
                        string msg = list.Find(p => p.Type == "MSG" && p.Key == "KJCONTENT").Value;

                        frmMainForm.PerioidString = resultArray;
                        msg = msg.Replace("[期号]", "期号：" + racingNum);
                        msg = msg.Replace("[开奖结果]", resultArray);


                        //获取本期参与押注的人员
                        List<NowMsg> msgList = new List<NowMsg>();
                        msgList = data.GetList<NowMsg>(" isdelete=2 and CommandType in ('买名次','冠亚和','名次大小单双龙虎') and period= " + frmMainForm.Perioid, "");
                        List<string> listUin = msgList.Select(p => p.MsgFromId).Distinct().ToList();

                        if (listUin != null && listUin.Count > 0)
                        {
                            string url = "/members/stake";
                            string strList = string.Join(",", listUin);
                            //strList = "[" + strList + "]";

                            string authStake = PanKou.Instance.GetSha1("", url + frmMainForm.Perioid + strList);
                            Log.WriteLogByDate("开始请求下注结果");
                            //请求开奖结果
                            string jsonStake = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + url + "?racingNum=" + frmMainForm.Perioid + "&wechatSns=" + strList, authStake, PanKou.accessKey);

                            Log.WriteLogByDate("请求结束下注结果是：" + jsonStake);

                            JObject jobject = JsonConvert.DeserializeObject(jsonStake) as JObject;

                            List<KaiJianBody> listBody = JsonConvert.DeserializeObject<List<KaiJianBody>>(jobject["data"].ToString());
                            //更新本地用户积分
                            foreach (KaiJianBody item in listBody)
                            {
                                //更新本地积分
                                if (item.members != null)
                                {
                                    data.ExecuteSql(string.Format(" update contactscore set totalScore={0} where uin={1}", item.members.points, item.members.wechatSn));

                                    msg += "\r\n" + item.members.nickName + "剩余积分：" + item.members.points;
                                    msg += "盈亏：" + item.memberStake.totalDeficitAmount;
                                }
                            }

                        }

                        Log.WriteLogByDate("开始发送开奖结果消息");
                        frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = msg, To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);
                        Log.WriteLogByDate("结束发送开奖结果消息");
                        frmMainForm.IsContinue = true;
                        frmMainForm.IsKaiJian = false;
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLogByDate("开奖结果消息：");
                    Log.WriteLog(ex);
                }
            }

        }

        public static void NewMethod(DataHelper data, List<NowMsg> msgList, string fromId)
        {
            StakeVoMax vo = new StakeVoMax();
            vo.wechatSn = fromId;
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

            List<NowMsg> msgMinList = msgList.Where(p => p.MsgFromId == fromId).ToList();
            foreach (NowMsg item in msgMinList)
            {
                vo2.racingNum = item.Period;
                //'买名次','冠亚和','名次大小单双龙虎'
                NewMethod1(vo2, item);

                data.ExecuteSql("update Nowmsg set isdelete='2' where msgid=" + item.MsgId);
            }

            string url = "/member/stake";

            string body = JsonConvert.SerializeObject(vo);
            body = body.Replace(" ", "");
            body = Regex.Replace(body, "\\s{2,}", ",");
            body = "[" + body + "]";
            string auth = PanKou.Instance.GetSha1(body, url);

            Log.WriteLogByDate(body);
            Log.WriteLogByDate(PanKou.accessKey);
            //请求押注接口
            string json = WebService.SendPostRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + url, body, auth, PanKou.accessKey);

            //查看结果
            Log.WriteLogByDate("押注接口：" + json);
        }

        public static void NewMethod1(StakeVo vo2, NowMsg item)
        {
            switch (item.CommandType)
            {
                case "买名次":
                    Log.WriteLogByDate("买名次");
                    item.CommandOne = item.CommandOne.Replace("冠", "1");
                    item.CommandOne = item.CommandOne.Replace("亚", "2");
                    item.CommandOne = item.CommandOne.Replace("季", "3");
                    foreach (char comdTwo in item.CommandTwo)
                    {
                        switch (comdTwo)
                        {
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':

                                foreach (char commandone1 in item.CommandOne)
                                {
                                    string commandone2 = commandone1.ToString();
                                    switch (commandone2)
                                    {
                                        case "1":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].first = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].first + Convert.ToInt32(item.Score);
                                            break;
                                        case "2":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].second = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].second + Convert.ToInt32(item.Score);
                                            break;
                                        case "3":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].third = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].third + Convert.ToInt32(item.Score);
                                            break;
                                        case "4":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].fourth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].fourth + Convert.ToInt32(item.Score);
                                            break;

                                        case "5":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].fifth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].fifth + Convert.ToInt32(item.Score);
                                            break;
                                        case "6":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].sixth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].sixth + Convert.ToInt32(item.Score);
                                            break;
                                        case "7":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].seventh = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].seventh + Convert.ToInt32(item.Score);
                                            break;
                                        case "8":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].eighth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].eighth + Convert.ToInt32(item.Score);
                                            break;
                                        case "9":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].ninth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].ninth + Convert.ToInt32(item.Score);
                                            break;
                                        case "0":
                                            vo2.appointStakeList[9].tenth = vo2.appointStakeList[9].tenth + Convert.ToInt32(item.Score);
                                            break;
                                    }
                                }
                                break;
                            case '0':
                                item.CommandOne = item.CommandOne.Replace("冠", "1");
                                item.CommandOne = item.CommandOne.Replace("亚", "2");
                                item.CommandOne = item.CommandOne.Replace("季", "3");
                                foreach (char commandone1 in item.CommandOne)
                                {
                                    string commandone2 = commandone1.ToString();
                                    switch (commandone2)
                                    {
                                        case "1":
                                            vo2.appointStakeList[9].first = vo2.appointStakeList[9].first + Convert.ToInt32(item.Score);
                                            break;
                                        case "2":
                                            vo2.appointStakeList[9].second = vo2.appointStakeList[9].second + Convert.ToInt32(item.Score);
                                            break;
                                        case "3":
                                            vo2.appointStakeList[9].third = vo2.appointStakeList[9].third + Convert.ToInt32(item.Score);
                                            break;
                                        case "4":
                                            vo2.appointStakeList[9].fourth = vo2.appointStakeList[9].fourth + Convert.ToInt32(item.Score);
                                            break;

                                        case "5":
                                            vo2.appointStakeList[9].fifth = vo2.appointStakeList[9].fifth + Convert.ToInt32(item.Score);
                                            break;
                                        case "6":
                                            vo2.appointStakeList[9].sixth = vo2.appointStakeList[9].sixth + Convert.ToInt32(item.Score);
                                            break;
                                        case "7":
                                            vo2.appointStakeList[9].seventh = vo2.appointStakeList[9].seventh + Convert.ToInt32(item.Score);
                                            break;
                                        case "8":
                                            vo2.appointStakeList[9].eighth = vo2.appointStakeList[9].eighth + Convert.ToInt32(item.Score);
                                            break;
                                        case "9":
                                            vo2.appointStakeList[9].ninth = vo2.appointStakeList[9].ninth + Convert.ToInt32(item.Score);
                                            break;
                                        case "0":
                                            vo2.appointStakeList[9].tenth = vo2.appointStakeList[9].tenth + Convert.ToInt32(item.Score);
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "冠亚和":
                    Log.WriteLogByDate("冠亚和");
                    string comTwo = item.CommandTwo.Replace("/", "");
                    if (Convert.ToInt32(comTwo) <= 19)
                    {
                        switch (comTwo)
                        {
                            case "3":
                                vo2.commonStake.firstSecond3 = vo2.commonStake.firstSecond3 + Convert.ToInt32(item.Score);
                                break;
                            case "4":
                                vo2.commonStake.firstSecond4 = vo2.commonStake.firstSecond4 + Convert.ToInt32(item.Score);
                                break;
                            case "5":
                                vo2.commonStake.firstSecond5 = vo2.commonStake.firstSecond5 + Convert.ToInt32(item.Score);
                                break;
                            case "6":
                                vo2.commonStake.firstSecond6 = vo2.commonStake.firstSecond6 + Convert.ToInt32(item.Score);
                                break;
                            case "7":
                                vo2.commonStake.firstSecond7 = vo2.commonStake.firstSecond7 + Convert.ToInt32(item.Score);
                                break;
                            case "8":
                                vo2.commonStake.firstSecond8 = vo2.commonStake.firstSecond8 + Convert.ToInt32(item.Score);
                                break;
                            case "9":
                                vo2.commonStake.firstSecond9 = vo2.commonStake.firstSecond9 + Convert.ToInt32(item.Score);
                                break;
                            case "10":
                                vo2.commonStake.firstSecond10 = vo2.commonStake.firstSecond10 + Convert.ToInt32(item.Score);
                                break;
                            case "11":
                                vo2.commonStake.firstSecond11 = vo2.commonStake.firstSecond11 + Convert.ToInt32(item.Score);
                                break;
                            case "12":
                                vo2.commonStake.firstSecond12 = vo2.commonStake.firstSecond12 + Convert.ToInt32(item.Score);
                                break;
                            case "13":
                                vo2.commonStake.firstSecond13 = vo2.commonStake.firstSecond13 + Convert.ToInt32(item.Score);
                                break;
                            case "14":
                                vo2.commonStake.firstSecond14 = vo2.commonStake.firstSecond14 + Convert.ToInt32(item.Score);
                                break;
                            case "15":
                                vo2.commonStake.firstSecond15 = vo2.commonStake.firstSecond15 + Convert.ToInt32(item.Score);
                                break;
                            case "16":
                                vo2.commonStake.firstSecond16 = vo2.commonStake.firstSecond16 + Convert.ToInt32(item.Score);
                                break;
                            case "17":
                                vo2.commonStake.firstSecond17 = vo2.commonStake.firstSecond17 + Convert.ToInt32(item.Score);
                                break;
                            case "18":
                                vo2.commonStake.firstSecond18 = vo2.commonStake.firstSecond18 + Convert.ToInt32(item.Score);
                                break;
                            case "19":
                                vo2.commonStake.firstSecond19 = vo2.commonStake.firstSecond19 + Convert.ToInt32(item.Score);
                                break;
                        }
                    }
                    else
                    {
                        //多条和指令
                        //foreach (char comTwoIn in comTwo)
                        //{
                        //    string commanTwo = comTwoIn.ToString();
                        //    switch (commanTwo)
                        //    {
                        //        case "3":
                        //            vo2.commonStake.firstSecond3 = vo2.commonStake.firstSecond3 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "4":
                        //            vo2.commonStake.firstSecond4 = vo2.commonStake.firstSecond4 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "5":
                        //            vo2.commonStake.firstSecond5 = vo2.commonStake.firstSecond5 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "6":
                        //            vo2.commonStake.firstSecond6 = vo2.commonStake.firstSecond6 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "7":
                        //            vo2.commonStake.firstSecond7 = vo2.commonStake.firstSecond7 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "8":
                        //            vo2.commonStake.firstSecond8 = vo2.commonStake.firstSecond8 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "9":
                        //            vo2.commonStake.firstSecond9 = vo2.commonStake.firstSecond9 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "10":
                        //            vo2.commonStake.firstSecond10 = vo2.commonStake.firstSecond10 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "11":
                        //            vo2.commonStake.firstSecond11 = vo2.commonStake.firstSecond11 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "12":
                        //            vo2.commonStake.firstSecond12 = vo2.commonStake.firstSecond12 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "13":
                        //            vo2.commonStake.firstSecond13 = vo2.commonStake.firstSecond13 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "14":
                        //            vo2.commonStake.firstSecond14 = vo2.commonStake.firstSecond14 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "15":
                        //            vo2.commonStake.firstSecond15 = vo2.commonStake.firstSecond15 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "16":
                        //            vo2.commonStake.firstSecond16 = vo2.commonStake.firstSecond16 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "17":
                        //            vo2.commonStake.firstSecond17 = vo2.commonStake.firstSecond17 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "18":
                        //            vo2.commonStake.firstSecond18 = vo2.commonStake.firstSecond18 + Convert.ToInt32(item.Score);
                        //            break;
                        //        case "19":
                        //            vo2.commonStake.firstSecond19 = vo2.commonStake.firstSecond19 + Convert.ToInt32(item.Score);
                        //            break;
                        //    }
                        //}
                    }
                    break;
                case "名次大小单双龙虎":
                    Log.WriteLogByDate("名次大小单双龙虎");
                    string comTwo3 = item.CommandTwo.Replace("/", "");
                    switch (item.CommandOne)
                    {
                        case "大":
                            foreach (char comdTwo in item.CommandTwo)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + Convert.ToInt32(item.Score);
                                        break;
                                    case '2':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + Convert.ToInt32(item.Score);
                                        break;
                                    case '3':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + Convert.ToInt32(item.Score);
                                        break;
                                    case '4':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + Convert.ToInt32(item.Score);
                                        break;
                                    case '5':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + Convert.ToInt32(item.Score);
                                        break;
                                    case '6':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + Convert.ToInt32(item.Score);
                                        break;
                                    case '7':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + Convert.ToInt32(item.Score);
                                        break;
                                    case '8':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + Convert.ToInt32(item.Score);
                                        break;
                                    case '9':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + Convert.ToInt32(item.Score);
                                        break;
                                    case '0':
                                        vo2.rankingStakeList[9].big = vo2.rankingStakeList[9].big + Convert.ToInt32(item.Score);
                                        break;
                                }
                            }
                            break;
                        case "小":
                            foreach (char comdTwo in item.CommandTwo)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + Convert.ToInt32(item.Score);
                                        break;
                                    case '2':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + Convert.ToInt32(item.Score);
                                        break;
                                    case '3':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + Convert.ToInt32(item.Score);
                                        break;
                                    case '4':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + Convert.ToInt32(item.Score);
                                        break;
                                    case '5':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + Convert.ToInt32(item.Score);
                                        break;
                                    case '6':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + Convert.ToInt32(item.Score);
                                        break;
                                    case '7':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + Convert.ToInt32(item.Score);
                                        break;
                                    case '8':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + Convert.ToInt32(item.Score);
                                        break;
                                    case '9':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + Convert.ToInt32(item.Score);
                                        break;
                                    case '0':
                                        vo2.rankingStakeList[9].small = vo2.rankingStakeList[9].small + Convert.ToInt32(item.Score);
                                        break;
                                }
                            }
                            break;
                        case "单":
                            foreach (char comdTwo in item.CommandTwo)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + Convert.ToInt32(item.Score);
                                        break;
                                    case '2':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + Convert.ToInt32(item.Score);
                                        break;
                                    case '3':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + Convert.ToInt32(item.Score);
                                        break;
                                    case '4':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + Convert.ToInt32(item.Score);
                                        break;
                                    case '5':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + Convert.ToInt32(item.Score);
                                        break;
                                    case '6':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + Convert.ToInt32(item.Score);
                                        break;
                                    case '7':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + Convert.ToInt32(item.Score);
                                        break;
                                    case '8':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + Convert.ToInt32(item.Score);
                                        break;
                                    case '9':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + Convert.ToInt32(item.Score);
                                        break;
                                    case '0':
                                        vo2.rankingStakeList[9].odd = vo2.rankingStakeList[9].odd + Convert.ToInt32(item.Score);
                                        break;
                                }
                            }

                            break;
                        case "双":
                            foreach (char comdTwo in item.CommandTwo)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + Convert.ToInt32(item.Score);
                                        break;
                                    case '2':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + Convert.ToInt32(item.Score);
                                        break;
                                    case '3':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + Convert.ToInt32(item.Score);
                                        break;
                                    case '4':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + Convert.ToInt32(item.Score);
                                        break;
                                    case '5':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + Convert.ToInt32(item.Score);
                                        break;
                                    case '6':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + Convert.ToInt32(item.Score);
                                        break;
                                    case '7':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + Convert.ToInt32(item.Score);
                                        break;
                                    case '8':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + Convert.ToInt32(item.Score);
                                        break;
                                    case '9':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + Convert.ToInt32(item.Score);
                                        break;
                                    case '0':
                                        vo2.rankingStakeList[9].even = vo2.rankingStakeList[9].even + Convert.ToInt32(item.Score);
                                        break;
                                }
                            }
                            break;
                        case "龙":
                            foreach (char comdTwo in comTwo3)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.commonStake.firstUp = vo2.commonStake.firstUp + Convert.ToInt32(item.Score);
                                        break;
                                    case '2':
                                        vo2.commonStake.secondUp = vo2.commonStake.secondUp + Convert.ToInt32(item.Score);
                                        break;
                                    case '3':
                                        vo2.commonStake.thirdUp = vo2.commonStake.thirdUp + Convert.ToInt32(item.Score);
                                        break;
                                    case '4':
                                        vo2.commonStake.fourthUp = vo2.commonStake.fourthUp + Convert.ToInt32(item.Score);
                                        break;
                                    case '5':
                                        vo2.commonStake.fifthUp = vo2.commonStake.fifthUp + Convert.ToInt32(item.Score);
                                        break;
                                }
                            }
                            break;
                        case "虎":
                            foreach (char comdTwo in comTwo3)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.commonStake.firstDowm = vo2.commonStake.firstDowm + Convert.ToInt32(item.Score);
                                        break;
                                    case '2':
                                        vo2.commonStake.secondDowm = vo2.commonStake.secondDowm + Convert.ToInt32(item.Score);
                                        break;
                                    case '3':
                                        vo2.commonStake.thirdDowm = vo2.commonStake.thirdDowm + Convert.ToInt32(item.Score);
                                        break;
                                    case '4':
                                        vo2.commonStake.fourthDowm = vo2.commonStake.fourthDowm + Convert.ToInt32(item.Score);
                                        break;
                                    case '5':
                                        vo2.commonStake.fifthDowm = vo2.commonStake.fifthDowm + Convert.ToInt32(item.Score);
                                        break;
                                }
                            }
                            break;
                    }

                    break;
                default:
                    Log.WriteLogByDate("押注信息上传错误指令：不应该出现" + item.CommandType);
                    break;
            }
        }
    }
}
