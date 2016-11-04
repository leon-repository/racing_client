using BLL;
using DrawTool;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using WxGames.Body;

namespace WxGames
{
    public class Lobby
    {
        private Lobby() { }

        public static readonly Lobby Instance = new Lobby();

        public void Start()
        {
            try
            {
                Begin();
            }
            catch (Exception ex)
            {
                Log.WriteLogByDate("获取开奖信息功能失败，原因是：");
                Log.WriteLog(ex);
            }
        }

        private static void Begin()
        {
            DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());
            //获取开奖信息，并将开奖信息保存到数据库
            string urlConfiger = "/user/client/stake/configer";
            string auth = PanKou.Instance.GetSha1("", urlConfiger);

            string json = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger, auth, PanKou.accessKey);
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
            if (stage == "1")
            {
                frmMainForm.IsJieDan = true;

                frmMainForm.Perioid = gameId;
                frmMainForm.IsFengPan = false;
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

                        GameMsg gameMsgFpx = new GameMsg();
                        gameMsgFpx.Uuid = Guid.NewGuid().ToString();
                        gameMsgFpx.GameId = game.GameId;
                        gameMsgFpx.SendTime = -1;
                        gameMsgFpx.Content = list.Find(p => p.Type == "MSG" && p.Key == "FPX").Value;
                        gameMsgFpx.IsSend = "0";
                        data.Insert<GameMsg>(gameMsgFpx, "");
                    }
                }
            }

            //获取未发送的消息
            List<GameMsg> listMsg = data.GetList<GameMsg>(" isSend='0' ", "");

            foreach (GameMsg msg in listMsg)
            {
                if (msg.SendTime == -1)
                {
                    continue;
                }

                if (msg.SendTime <= DateTime.Now.DateTimeToUnixTimestamp())
                {
                    //先检查有没有未处理完的信息
                    //有的话，跳过
                    string content = msg.Content;
                    if (content.Contains("[下注信息]"))
                    {
                        List<NowMsg> nowMsgList = data.GetList<NowMsg>(" CommandType not in ('上下查') and isdelete='0' ", "");
                        if (nowMsgList.Count > 0)
                        {
                            continue;
                        }
                    }

                    data.ExecuteSql(string.Format(" update gamemsg set issend='1' where uuid='{0}' ", msg.Uuid));


                    //查找下注信息
                    if (content.Contains("[下注信息]"))
                    {
                        //生成下注信息
                        try
                        {
                            content = msg.Content.Replace("[下注信息]", "");
                            List<NowMsg> nowMsgList = data.GetList<NowMsg>(string.Format(" CommandType not in ('上下查','下注积分范围错误','取消','指令格式错误') and isdelete='2' and period='{0}' ", frmMainForm.Perioid), "");
                            List<string> listUin = nowMsgList.Select(p => p.MsgFromId).Distinct().ToList();
                            foreach (string uin in listUin)
                            {
                                List<NowMsg> listUinMsg = nowMsgList.Where(p => p.MsgFromId == uin).ToList();
                                string msgMessage = "";
                                msgMessage += "[" + listUinMsg.Find(p => p.MsgFromId == uin).MsgFromName + "]";
                                msgMessage += "[";
                                foreach (var message in listUinMsg)
                                {
                                    msgMessage += message.OrderContect + "#";
                                }
                                msgMessage += "]\r\n";
                                content += msgMessage;

                                data.ExecuteSql(string.Format("update contactscore set runscore=0 where uin={0}", uin));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLogByDate("获取下注信息出错");
                            Log.WriteLog(ex);
                        }
                    }
                    else if (content.Contains("[历史]"))
                    {
                        try
                        {
                            frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = "历史开奖图", To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);

                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog(ex);
                        }
                        //生成图片，并发送
                        string urlConfiger2 = "/racing/web/history";
                        string json2 = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger2, "", PanKou.accessKey);
                        string strJson = json2;
                        DrawImage image = new DrawImage(1400, 700);
                        image.SetSavePath(AppDomain.CurrentDomain.BaseDirectory + "\\DramImage.png");
                        image.SetFramePen(Color.Gray);
                        image.SetNumberBackgroundColor(1, Color.FromArgb(245, 245, 245));
                        image.SetNumberBackgroundColor(2, Color.FromArgb(249, 140, 21));
                        image.SetNumberBackgroundColor(3, Color.FromArgb(40, 83, 141));
                        image.SetNumberBackgroundColor(4, Color.FromArgb(251, 227, 24));
                        image.SetNumberBackgroundColor(5, Color.FromArgb(102, 102, 102));
                        image.SetNumberBackgroundColor(6, Color.FromArgb(41, 134, 73));
                        image.SetNumberBackgroundColor(7, Color.FromArgb(162, 163, 164));
                        image.SetNumberBackgroundColor(8, Color.FromArgb(60, 214, 233));
                        image.SetNumberBackgroundColor(9, Color.FromArgb(224, 57, 58));
                        image.SetNumberBackgroundColor(10, Color.FromArgb(46, 52, 180));
                        JArray jData = new JArray();
                        jData = DataFormat.FormatString(strJson);
                        image.Draw(jData);
                        image.Save();
                        FileInfo file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\DramImage.png");
                        frmMainForm.wxs.SendImage("DramImage.png", AppDomain.CurrentDomain.BaseDirectory + "\\DramImage.png", file.Length.ToString(), frmMainForm.CurrentWX.UserName, frmMainForm.CurrentQun, Log.GetMD5HashFromFile(AppDomain.CurrentDomain.BaseDirectory + "\\DramImage.png"));

                        content = content.Replace("[历史]", "");
                    }
                    if (content.Contains("[冠军走势]"))
                    {
                        string url2 = "/user/client/history/champion";
                        string authStake2 = PanKou.Instance.GetSha1("" + 10, url2);
                        string jsonStake2 = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + url2 + "?nper=10", authStake2, PanKou.accessKey);

                        if (!string.IsNullOrEmpty(jsonStake2))
                        {
                            JObject jobject = JsonConvert.DeserializeObject(jsonStake2) as JObject;
                            if (jobject != null)
                            {
                                content = content.Replace("[冠军走势]", "冠军走势：" + jobject["message"].ToString());
                            }
                        }
                    }

                    frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = content, To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);
                }
            }

            //判断阶段
            //上报阶段 下注信息上传
            if (stage == "2")
            {
                frmMainForm.IsJieDan = false;
                frmMainForm.IsFengPan = true;
                List<NowMsg> msgList = new List<NowMsg>();


                List<GameMsg> listFpx = data.GetList<GameMsg>(" isSend='0' and SendTime='-1' ", "");

                if (listFpx.Count > 0)
                {
                    //发送封盘线
                    frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = frmMainForm.Configs.Find(p => p.Key == "FPX").Value, To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);
                    data.ExecuteSql(" update gamemsg set issend='1' where isSend='0' and SendTime='-1'");
                }

                msgList = data.GetList<NowMsg>(" isdelete=1 and CommandType in ('买名次','冠亚和','名次大小单双龙虎','和大','和小','和单','和双') and period= " + frmMainForm.Perioid, "");
                //下注信息解析成押注信息上传到服务器

                if (msgList.Count > 0)
                {
                    List<string> msgFromIdList = msgList.Select(p => p.MsgFromId).Distinct<string>().ToList();
                    List<StakeVoMax> list = new List<StakeVoMax>();
                    foreach (string fromId in msgFromIdList)
                    {
                        if (string.IsNullOrEmpty(fromId) || fromId == "0")
                        {
                            continue;
                        }
                        try
                        {
                            list.Add(NewMethod(data, msgList, fromId));
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLogByDate("上报阶段异常");
                            Log.WriteLog(ex);
                        }
                    }

                    try
                    {
                        string url = "/member/stake";
                        string body = JsonConvert.SerializeObject(list);
                        body = body.Replace(" ", "");
                        body = Regex.Replace(body, "\\s{2,}", ",");
                        string auth3 = PanKou.Instance.GetSha1(body, url);
                        Log.WriteLogByDate("body:" + body);
                        //请求押注接口
                        string json3 = WebService.SendPostRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + url, body, auth3, PanKou.accessKey);

                        //查看结果
                        Log.WriteLogByDate("押注接口：" + json3);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLogByDate("押注错误");
                        Log.WriteLog(ex);
                    }

                }
                frmMainForm.IsKaiJian = true;
                frmMainForm.IsComplete = false;
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

                    string json2 = "";

                    while (string.IsNullOrEmpty(json2))
                    {
                        json2 = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger2 + "?racingNum=" + frmMainForm.Perioid, auth2, PanKou.accessKey);
                        Thread.Sleep(1000);
                    }

                    if (string.IsNullOrEmpty(json2))
                    {
                        frmMainForm.IsKaiJian = false;
                        frmMainForm.IsFengPan = false;

                        frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = "----接收下单---", To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);
                        frmMainForm.IsComplete = true;

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
                        string strYinKui = "";
                        List<NowMsg> msgList = new List<NowMsg>();
                        msgList = data.GetList<NowMsg>(" isdelete=2 and CommandType in ('买名次','冠亚和','名次大小单双龙虎') and period= " + frmMainForm.Perioid, "");
                        List<string> listUin = msgList.Select(p => p.MsgFromId).Distinct().ToList();

                        if (listUin != null && listUin.Count > 0)
                        {
                            string url = "/members/stake";
                            string strList = string.Join(",", listUin);

                            string authStake = PanKou.Instance.GetSha1("", url + frmMainForm.Perioid + strList);
                            //请求开奖结果
                            string jsonStake = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + url + "?racingNum=" + frmMainForm.Perioid + "&wechatSns=" + strList, authStake, PanKou.accessKey);

                            Log.WriteLogByDate("开奖结果是：" + jsonStake);
                            if (string.IsNullOrEmpty(jsonStake))
                            {

                            }
                            else
                            {
                                JObject jobject = JsonConvert.DeserializeObject(jsonStake) as JObject;

                                List<KaiJianBody> listBody = JsonConvert.DeserializeObject<List<KaiJianBody>>(jobject["data"].ToString());
                                //更新本地用户积分
                                foreach (KaiJianBody item in listBody)
                                {
                                    //更新本地积分
                                    if (item.members != null)
                                    {
                                        data.ExecuteSql(string.Format(" update contactscore set totalScore={0} where uin={1}", item.members.points, item.members.wechatSn));

                                        strYinKui += "[" + item.members.nickName + "][剩余积分：" + item.members.points;
                                        strYinKui += "][盈亏：" + item.memberStake.totalDeficitAmount + "]\r\n";
                                    }
                                }

                                msg = msg.Replace("[盈亏]", strYinKui);
                            }
                        }

                        frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = msg, To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);

                        //把所有未处理的指令，全部失效
                        data.ExecuteSql(" update OriginMsg set issucc='9' where issucc=0 ");
                        data.ExecuteSql(" update NowMsg set issucc=2 where issucc=1 ");

                        frmMainForm.IsKaiJian = false;
                        frmMainForm.IsFengPan = false;

                        frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = "----接收下单---", To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);
                        frmMainForm.IsComplete = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLogByDate("开奖结果消息：");
                    Log.WriteLog(ex);
                }
            }
        }

        public static StakeVoMax NewMethod(DataHelper data, List<NowMsg> msgList, string fromId)
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
                try
                {
                    vo2.racingNum = item.Period;
                    //'买名次','冠亚和','名次大小单双龙虎','和大','和小','和单','和双'
                    NewMethod1(vo2, item);

                    data.ExecuteSql(string.Format("update Nowmsg set isdelete='2' where msgid='{0}'", item.MsgId));
                }
                catch (Exception ex)
                {
                    //发生异常，删除消息
                    data.ExecuteSql(string.Format("delete from Nowmsg where msgid='{0}'",item.MsgId));
                }
            }


            return vo;
        }

        public static void NewMethod1(StakeVo vo2, NowMsg item)
        {
            switch (item.CommandType)
            {
                case "买名次":
                    item.CommandOne = item.CommandOne.Replace("冠", "1");
                    item.CommandOne = item.CommandOne.Replace("亚", "2");
                    item.CommandOne = item.CommandOne.Replace("季", "3");

                    item.CommandOne = item.CommandOne.Replace("一", "1");
                    item.CommandOne = item.CommandOne.Replace("二", "2");
                    item.CommandOne = item.CommandOne.Replace("三", "3");
                    item.CommandOne = item.CommandOne.Replace("四", "4");
                    item.CommandOne = item.CommandOne.Replace("五", "5");
                    item.CommandOne = item.CommandOne.Replace("六", "6");
                    item.CommandOne = item.CommandOne.Replace("七", "7");
                    item.CommandOne = item.CommandOne.Replace("八", "8");
                    item.CommandOne = item.CommandOne.Replace("九", "9");
                    item.CommandOne = item.CommandOne.Replace("十", "0");

                    //如果依然存在汉字，就是有问题，break掉
                    if (item.CommandOne.ExitHanZi())
                    {
                        break;
                    }

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
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].first = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].first + item.Score.ToInt();
                                            break;
                                        case "2":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].second = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].second + item.Score.ToInt();
                                            break;
                                        case "3":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].third = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].third + item.Score.ToInt();
                                            break;
                                        case "4":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].fourth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].fourth + item.Score.ToInt();
                                            break;

                                        case "5":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].fifth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].fifth + item.Score.ToInt();
                                            break;
                                        case "6":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].sixth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].sixth + item.Score.ToInt();
                                            break;
                                        case "7":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].seventh = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].seventh + item.Score.ToInt();
                                            break;
                                        case "8":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].eighth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].eighth + item.Score.ToInt();
                                            break;
                                        case "9":
                                            vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].ninth = vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].ninth + item.Score.ToInt();
                                            break;
                                        case "0":
                                            if (comdTwo.ToString() == "0")
                                            {
                                                vo2.appointStakeList[9].tenth = vo2.appointStakeList[9].tenth + item.Score.ToInt();
                                            }
                                            else
                                            {
                                                vo2.appointStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].tenth = vo2.appointStakeList[9].tenth + item.Score.ToInt();

                                            }

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
                                            vo2.appointStakeList[9].first = vo2.appointStakeList[9].first + item.Score.ToInt();
                                            break;
                                        case "2":
                                            vo2.appointStakeList[9].second = vo2.appointStakeList[9].second + item.Score.ToInt();
                                            break;
                                        case "3":
                                            vo2.appointStakeList[9].third = vo2.appointStakeList[9].third + item.Score.ToInt();
                                            break;
                                        case "4":
                                            vo2.appointStakeList[9].fourth = vo2.appointStakeList[9].fourth + item.Score.ToInt();
                                            break;

                                        case "5":
                                            vo2.appointStakeList[9].fifth = vo2.appointStakeList[9].fifth + item.Score.ToInt();
                                            break;
                                        case "6":
                                            vo2.appointStakeList[9].sixth = vo2.appointStakeList[9].sixth + item.Score.ToInt();
                                            break;
                                        case "7":
                                            vo2.appointStakeList[9].seventh = vo2.appointStakeList[9].seventh + item.Score.ToInt();
                                            break;
                                        case "8":
                                            vo2.appointStakeList[9].eighth = vo2.appointStakeList[9].eighth + item.Score.ToInt();
                                            break;
                                        case "9":
                                            vo2.appointStakeList[9].ninth = vo2.appointStakeList[9].ninth + item.Score.ToInt();
                                            break;
                                        case "0":
                                            vo2.appointStakeList[9].tenth = vo2.appointStakeList[9].tenth + item.Score.ToInt();
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "冠亚和":
                    string comTwo = item.CommandTwo.Replace("/", "");
                    if (comTwo.Length <= 2 && Convert.ToInt32(comTwo) <= 19)
                    {
                        switch (comTwo)
                        {
                            case "3":
                                vo2.commonStake.firstSecond3 = vo2.commonStake.firstSecond3 + item.Score.ToInt();
                                break;
                            case "4":
                                vo2.commonStake.firstSecond4 = vo2.commonStake.firstSecond4 + item.Score.ToInt();
                                break;
                            case "5":
                                vo2.commonStake.firstSecond5 = vo2.commonStake.firstSecond5 + item.Score.ToInt();
                                break;
                            case "6":
                                vo2.commonStake.firstSecond6 = vo2.commonStake.firstSecond6 + item.Score.ToInt();
                                break;
                            case "7":
                                vo2.commonStake.firstSecond7 = vo2.commonStake.firstSecond7 + item.Score.ToInt();
                                break;
                            case "8":
                                vo2.commonStake.firstSecond8 = vo2.commonStake.firstSecond8 + item.Score.ToInt();
                                break;
                            case "9":
                                vo2.commonStake.firstSecond9 = vo2.commonStake.firstSecond9 + item.Score.ToInt();
                                break;
                            case "10":
                                vo2.commonStake.firstSecond10 = vo2.commonStake.firstSecond10 + item.Score.ToInt();
                                break;
                            case "11":
                                vo2.commonStake.firstSecond11 = vo2.commonStake.firstSecond11 + item.Score.ToInt();
                                break;
                            case "12":
                                vo2.commonStake.firstSecond12 = vo2.commonStake.firstSecond12 + item.Score.ToInt();
                                break;
                            case "13":
                                vo2.commonStake.firstSecond13 = vo2.commonStake.firstSecond13 + item.Score.ToInt();
                                break;
                            case "14":
                                vo2.commonStake.firstSecond14 = vo2.commonStake.firstSecond14 + item.Score.ToInt();
                                break;
                            case "15":
                                vo2.commonStake.firstSecond15 = vo2.commonStake.firstSecond15 + item.Score.ToInt();
                                break;
                            case "16":
                                vo2.commonStake.firstSecond16 = vo2.commonStake.firstSecond16 + item.Score.ToInt();
                                break;
                            case "17":
                                vo2.commonStake.firstSecond17 = vo2.commonStake.firstSecond17 + item.Score.ToInt();
                                break;
                            case "18":
                                vo2.commonStake.firstSecond18 = vo2.commonStake.firstSecond18 + item.Score.ToInt();
                                break;
                            case "19":
                                vo2.commonStake.firstSecond19 = vo2.commonStake.firstSecond19 + item.Score.ToInt();
                                break;
                        }
                    }
                    else
                    {
                        //多条和指令
                        while (comTwo.Length > 0 && comTwo != "0" && comTwo != "1" && comTwo != "2")
                        {
                            char first = comTwo.First();

                            if (first.ToString().ToInt() >= 3 && first.ToString().ToInt() <= 9)
                            {
                                switch (first.ToString())
                                {
                                    case "3":
                                        vo2.commonStake.firstSecond3 = vo2.commonStake.firstSecond3 + item.Score.ToInt();
                                        break;
                                    case "4":
                                        vo2.commonStake.firstSecond4 = vo2.commonStake.firstSecond4 + item.Score.ToInt();
                                        break;
                                    case "5":
                                        vo2.commonStake.firstSecond5 = vo2.commonStake.firstSecond5 + item.Score.ToInt();
                                        break;
                                    case "6":
                                        vo2.commonStake.firstSecond6 = vo2.commonStake.firstSecond6 + item.Score.ToInt();
                                        break;
                                    case "7":
                                        vo2.commonStake.firstSecond7 = vo2.commonStake.firstSecond7 + item.Score.ToInt();
                                        break;
                                    case "8":
                                        vo2.commonStake.firstSecond8 = vo2.commonStake.firstSecond8 + item.Score.ToInt();
                                        break;
                                    case "9":
                                        vo2.commonStake.firstSecond9 = vo2.commonStake.firstSecond9 + item.Score.ToInt();
                                        break;
                                }

                                comTwo = string.Join("", comTwo.Reverse());
                                comTwo = comTwo.Remove(comTwo.Length - 1, 1);
                                comTwo = string.Join("", comTwo.Reverse());
                            }
                            else
                            {
                                if (comTwo.Length >= 2)
                                {
                                    string str = comTwo.Substring(0, 2);

                                    switch (str)
                                    {
                                        case "10":
                                            vo2.commonStake.firstSecond10 = vo2.commonStake.firstSecond10 + item.Score.ToInt();
                                            break;
                                        case "11":
                                            vo2.commonStake.firstSecond11 = vo2.commonStake.firstSecond11 + item.Score.ToInt();
                                            break;
                                        case "12":
                                            vo2.commonStake.firstSecond12 = vo2.commonStake.firstSecond12 + item.Score.ToInt();
                                            break;
                                        case "13":
                                            vo2.commonStake.firstSecond13 = vo2.commonStake.firstSecond13 + item.Score.ToInt();
                                            break;
                                        case "14":
                                            vo2.commonStake.firstSecond14 = vo2.commonStake.firstSecond14 + item.Score.ToInt();
                                            break;
                                        case "15":
                                            vo2.commonStake.firstSecond15 = vo2.commonStake.firstSecond15 + item.Score.ToInt();
                                            break;
                                        case "16":
                                            vo2.commonStake.firstSecond16 = vo2.commonStake.firstSecond16 + item.Score.ToInt();
                                            break;
                                        case "17":
                                            vo2.commonStake.firstSecond17 = vo2.commonStake.firstSecond17 + item.Score.ToInt();
                                            break;
                                        case "18":
                                            vo2.commonStake.firstSecond18 = vo2.commonStake.firstSecond18 + item.Score.ToInt();
                                            break;
                                        case "19":
                                            vo2.commonStake.firstSecond19 = vo2.commonStake.firstSecond19 + item.Score.ToInt();
                                            break;
                                    }

                                    comTwo = string.Join("", comTwo.Reverse());
                                    comTwo = comTwo.Remove(comTwo.Length - 2, 2);
                                    comTwo = string.Join("", comTwo.Reverse());
                                }
                            }
                        }
                    }
                    break;
                case "名次大小单双龙虎":
                    string comTwo3 = item.CommandTwo.Replace("/", "");
                    switch (comTwo3)
                    {
                        case "大":
                            foreach (char comdTwo in item.CommandOne)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + item.Score.ToInt();
                                        break;
                                    case '2':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + item.Score.ToInt();
                                        break;
                                    case '3':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + item.Score.ToInt();
                                        break;
                                    case '4':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + item.Score.ToInt();
                                        break;
                                    case '5':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + item.Score.ToInt();
                                        break;
                                    case '6':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + item.Score.ToInt();
                                        break;
                                    case '7':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + item.Score.ToInt();
                                        break;
                                    case '8':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + item.Score.ToInt();
                                        break;
                                    case '9':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].big + item.Score.ToInt();
                                        break;
                                    case '0':
                                        vo2.rankingStakeList[9].big = vo2.rankingStakeList[9].big + item.Score.ToInt();
                                        break;
                                }
                            }
                            break;
                        case "小":
                            foreach (char comdTwo in item.CommandOne)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + item.Score.ToInt();
                                        break;
                                    case '2':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + item.Score.ToInt();
                                        break;
                                    case '3':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + item.Score.ToInt();
                                        break;
                                    case '4':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + item.Score.ToInt();
                                        break;
                                    case '5':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + item.Score.ToInt();
                                        break;
                                    case '6':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + item.Score.ToInt();
                                        break;
                                    case '7':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + item.Score.ToInt();
                                        break;
                                    case '8':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + item.Score.ToInt();
                                        break;
                                    case '9':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].small + item.Score.ToInt();
                                        break;
                                    case '0':
                                        vo2.rankingStakeList[9].small = vo2.rankingStakeList[9].small + item.Score.ToInt();
                                        break;
                                }
                            }
                            break;
                        case "单":
                            foreach (char comdTwo in item.CommandOne)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + item.Score.ToInt();
                                        break;
                                    case '2':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + item.Score.ToInt();
                                        break;
                                    case '3':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + item.Score.ToInt();
                                        break;
                                    case '4':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + item.Score.ToInt();
                                        break;
                                    case '5':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + item.Score.ToInt();
                                        break;
                                    case '6':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + item.Score.ToInt();
                                        break;
                                    case '7':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + item.Score.ToInt();
                                        break;
                                    case '8':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + item.Score.ToInt();
                                        break;
                                    case '9':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].odd + item.Score.ToInt();
                                        break;
                                    case '0':
                                        vo2.rankingStakeList[9].odd = vo2.rankingStakeList[9].odd + item.Score.ToInt();
                                        break;
                                }
                            }

                            break;
                        case "双":
                            foreach (char comdTwo in item.CommandOne)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + item.Score.ToInt();
                                        break;
                                    case '2':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + item.Score.ToInt();
                                        break;
                                    case '3':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + item.Score.ToInt();
                                        break;
                                    case '4':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + item.Score.ToInt();
                                        break;
                                    case '5':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + item.Score.ToInt();
                                        break;
                                    case '6':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + item.Score.ToInt();
                                        break;
                                    case '7':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + item.Score.ToInt();
                                        break;
                                    case '8':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + item.Score.ToInt();
                                        break;
                                    case '9':
                                        vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even = vo2.rankingStakeList[Convert.ToInt32(comdTwo.ToString()) - 1].even + item.Score.ToInt();
                                        break;
                                    case '0':
                                        vo2.rankingStakeList[9].even = vo2.rankingStakeList[9].even + item.Score.ToInt();
                                        break;
                                }
                            }
                            break;
                        case "龙":
                            foreach (char comdTwo in item.CommandOne)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.commonStake.firstUp = vo2.commonStake.firstUp + item.Score.ToInt();
                                        break;
                                    case '2':
                                        vo2.commonStake.secondUp = vo2.commonStake.secondUp + item.Score.ToInt();
                                        break;
                                    case '3':
                                        vo2.commonStake.thirdUp = vo2.commonStake.thirdUp + item.Score.ToInt();
                                        break;
                                    case '4':
                                        vo2.commonStake.fourthUp = vo2.commonStake.fourthUp + item.Score.ToInt();
                                        break;
                                    case '5':
                                        vo2.commonStake.fifthUp = vo2.commonStake.fifthUp + item.Score.ToInt();
                                        break;
                                }
                            }
                            break;
                        case "虎":
                            foreach (char comdTwo in item.CommandOne)
                            {
                                switch (comdTwo)
                                {
                                    case '1':
                                        vo2.commonStake.firstDowm = vo2.commonStake.firstDowm + item.Score.ToInt();
                                        break;
                                    case '2':
                                        vo2.commonStake.secondDowm = vo2.commonStake.secondDowm + item.Score.ToInt();
                                        break;
                                    case '3':
                                        vo2.commonStake.thirdDowm = vo2.commonStake.thirdDowm + item.Score.ToInt();
                                        break;
                                    case '4':
                                        vo2.commonStake.fourthDowm = vo2.commonStake.fourthDowm + item.Score.ToInt();
                                        break;
                                    case '5':
                                        vo2.commonStake.fifthDowm = vo2.commonStake.fifthDowm + item.Score.ToInt();
                                        break;
                                }
                            }
                            break;
                    }
                    break;
                case "和大":
                    vo2.commonStake.firstSecondBig = vo2.commonStake.firstSecondBig + item.Score.ToInt();
                    break;
                case "和小":
                    vo2.commonStake.firstSecondSmall = vo2.commonStake.firstSecondSmall + item.Score.ToInt();
                    break;
                case "和单":
                    vo2.commonStake.firstSecondOdd = vo2.commonStake.firstSecondOdd + item.Score.ToInt();
                    break;
                case "和双":
                    vo2.commonStake.firstSecondEven = vo2.commonStake.firstSecondEven + item.Score.ToInt();
                    break;
                default:
                    Log.WriteLogByDate("押注信息上传错误指令：不应该出现" + item.CommandType);
                    break;
            }
        }
    }
}
