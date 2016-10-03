using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WxGames
{
    public class Lobby
    {
        private Lobby() { }

        public static readonly Lobby Instanc = new Lobby();

        public void Start()
        {
            DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());
            //获取开奖信息，并将开奖信息保存到数据库
            int gameId = 12345678;
            int gameNum = 7654321;

            //查看是否有开奖信息
            List<KeyValuePair<string, object>> pkListInit = new List<KeyValuePair<string, object>>();
            pkListInit.Add(new KeyValuePair<string, object>("IsSucc", 0));
            Game gameInit = data.First<Game>(pkListInit, "");
            if (gameInit == null)
            {
                Game game = new Game();
                game.Uuid = Guid.NewGuid().ToString();
                game.GameId = gameId.ToString();
                game.StartTime = DateTime.Now.DateTimeToUnixTimestamp().ToString();
                game.EndTime = (Convert.ToUInt64(game.StartTime) + 300).ToString();
                game.GameNum = gameNum.ToString();
                game.IsSucc = "0";
                List<KeyValuePair<string, object>> pkListInit2 = new List<KeyValuePair<string, object>>();
                pkListInit2.Add(new KeyValuePair<string, object>("gameId", gameId));
                Game gameInit2 = data.First<Game>(pkListInit2, "");
                if (gameInit2 == null)
                {
                    data.Insert<Game>(game, "");
                }
                else
                {
                    Game game2 = new Game();
                    game2.Uuid = Guid.NewGuid().ToString();
                    game2.GameId = data.GetSingle("select max(gameid) from game").ToString();
                    game2.StartTime = DateTime.Now.DateTimeToUnixTimestamp().ToString();
                    game2.EndTime = (Convert.ToUInt64(game.StartTime) + 300).ToString();
                    game2.GameNum = gameNum.ToString();
                    game2.IsSucc = "0";
                    data.Insert<Game>(game2, "");

                    //把以前的消息全修改为发送
                    data.ExecuteSql(" update GameMsg set issend='1' ");
                    List<Config> list = frmMainForm.Configs;
                    //向每期消息表插入信息
                    if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK3").Value))
                    {
                        //280秒
                        GameMsg gameMsg1 = new GameMsg();
                        gameMsg1.Uuid = Guid.NewGuid().ToString();
                        gameMsg1.GameId = game2.GameId;
                        gameMsg1.SendTime = game2.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "PTTIME3").Value.ToInt();
                        gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT3").Value;
                        gameMsg1.IsSend = "0";
                        data.Insert<GameMsg>(gameMsg1, "");
                    }

                    //110秒
                    if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK2").Value))
                    {
                        GameMsg gameMsg1 = new GameMsg();
                        gameMsg1.Uuid = Guid.NewGuid().ToString();
                        gameMsg1.GameId = game2.GameId;
                        gameMsg1.SendTime = game2.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "PTTIME2").Value.ToInt();
                        gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT2").Value;
                        gameMsg1.IsSend = "0";
                        data.Insert<GameMsg>(gameMsg1, "");
                    }

                    //封盘 60秒
                    if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "FPCHK").Value))
                    {
                        GameMsg gameMsg1 = new GameMsg();
                        gameMsg1.Uuid = Guid.NewGuid().ToString();
                        gameMsg1.GameId = game2.GameId;
                        gameMsg1.SendTime = game2.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "FPTIME").Value.ToInt();
                        gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "FPCONTENT").Value;
                        gameMsg1.IsSend = "0";
                        data.Insert<GameMsg>(gameMsg1, "");
                    }

                    //30秒
                    if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK").Value))
                    {
                        GameMsg gameMsg1 = new GameMsg();
                        gameMsg1.Uuid = Guid.NewGuid().ToString();
                        gameMsg1.GameId = game2.GameId;
                        gameMsg1.SendTime = game2.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "PTTIME").Value.ToInt();
                        gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT").Value;
                        gameMsg1.IsSend = "0";
                        data.Insert<GameMsg>(gameMsg1, "");
                    }
                }
            }
            else
            {
                List<Config> list = frmMainForm.Configs;
                //拿结束时间和现在时间做比较，如果小于现在时间，则重新生成一条开奖信息，并把之前的开奖信息发送
                if (Convert.ToInt64(gameInit.EndTime) <= DateTime.Now.DateTimeToUnixTimestamp())
                {
                    Game game2 = new Game();
                    game2.Uuid = Guid.NewGuid().ToString();
                    game2.GameId = (Convert.ToUInt32(gameInit.GameId) + 1).ToString();
                    game2.StartTime = DateTime.Now.DateTimeToUnixTimestamp().ToString();
                    game2.EndTime = (Convert.ToUInt64(game2.StartTime) + 300).ToString();
                    game2.GameNum = (Convert.ToUInt32(gameInit.GameNum) + 1).ToString();
                    game2.IsSucc = "0";

                    data.ExecuteSql(string.Format("update Game set IsSucc='1' where GameId='{0}' ", gameInit.GameId));
                    data.Insert<Game>(game2, "");

                    //把以前的消息全修改为发送
                    data.ExecuteSql(" update GameMsg set issend='1' ");

                    //向每期消息表插入信息
                    if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK3").Value))
                    {
                        //280秒
                        GameMsg gameMsg1 = new GameMsg();
                        gameMsg1.Uuid = Guid.NewGuid().ToString();
                        gameMsg1.GameId = game2.GameId;
                        gameMsg1.SendTime = game2.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "PTTIME3").Value.ToInt();
                        gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT3").Value;
                        gameMsg1.IsSend = "0";
                        data.Insert<GameMsg>(gameMsg1, "");
                    }

                    //110秒
                    if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK2").Value))
                    {
                        GameMsg gameMsg1 = new GameMsg();
                        gameMsg1.Uuid = Guid.NewGuid().ToString();
                        gameMsg1.GameId = game2.GameId;
                        gameMsg1.SendTime = game2.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "PTTIME2").Value.ToInt();
                        gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT2").Value;
                        gameMsg1.IsSend = "0";
                        data.Insert<GameMsg>(gameMsg1, "");
                    }

                    //封盘 60秒
                    if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "FPCHK").Value))
                    {
                        GameMsg gameMsg1 = new GameMsg();
                        gameMsg1.Uuid = Guid.NewGuid().ToString();
                        gameMsg1.GameId = game2.GameId;
                        gameMsg1.SendTime = game2.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "FPTIME").Value.ToInt();
                        gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "FPCONTENT").Value;
                        gameMsg1.IsSend = "0";
                        data.Insert<GameMsg>(gameMsg1, "");
                    }

                    //30秒
                    if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK").Value))
                    {
                        GameMsg gameMsg1 = new GameMsg();
                        gameMsg1.Uuid = Guid.NewGuid().ToString();
                        gameMsg1.GameId = game2.GameId;
                        gameMsg1.SendTime = game2.EndTime.ToInt() - list.Find(p => p.Type == "MSG" && p.Key == "PTTIME").Value.ToInt();
                        gameMsg1.Content = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT").Value;
                        gameMsg1.IsSend = "0";
                        data.Insert<GameMsg>(gameMsg1, "");
                    }

                    //发开奖信息，开奖结果图
                    if (Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "KJCHK").Value))
                    {
                        frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = list.Find(p => p.Type == "MSG" && p.Key == "KJCONTENT").Value, To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);

                        //发送图片
                    }
                }
            }

            //获取未发送的消息
            List<GameMsg> listMsg = data.GetList<GameMsg>(" isSend='0' ", "");

            foreach (GameMsg msg in listMsg)
            {
                if (msg.SendTime <= DateTime.Now.DateTimeToUnixTimestamp())
                {
                    data.ExecuteSql(string.Format(" update gamemsg set issend='1' where uuid='{0}' ",msg.Uuid));
                    frmMainForm.CurrentWX.SendMsg(new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg =msg.Content, To = frmMainForm.CurrentQun, Time = DateTime.Now, Type = 1, Readed = false }, false);
                }
            }
        }
    }
}
