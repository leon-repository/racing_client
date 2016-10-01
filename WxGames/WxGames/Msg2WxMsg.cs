using BLL;
using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WxGames
{
    /// <summary>
    /// 转化指令消息to微信消息
    /// </summary>
    public class Msg2WxMsg
    {
        public static readonly Msg2WxMsg Instance = new Msg2WxMsg();

        public WXMsg GetMsg(NowMsg msg)
        {
            string conn = ConfigurationManager.AppSettings["conn"].ToString();
            DataHelper data = new DataHelper(conn);

            WXMsg model = null;

            //处理消息
            if (msg == null)
            {
                return model;
            }

            if (msg.MsgFromId == null)
            {
                return model;
            }

            model = new WXMsg();

            List<KeyValuePair<string, object>> pkList = new List<KeyValuePair<string, object>>();
            pkList.Add(new KeyValuePair<string, object>("Uin", msg.MsgFromId));
            ContactScore contactScore = data.First<ContactScore>(pkList, "");
            if (contactScore == null)
            {
                contactScore = new ContactScore();
                contactScore.Uuid = Guid.NewGuid().ToString();
                contactScore.SyScore = 0;
                contactScore.TotalScore = 0;
                contactScore.RunScore = 0;
                contactScore.Uin = msg.MsgFromId;
                contactScore.NickName = msg.MsgFromName;
                data.Insert<ContactScore>(contactScore, "");
            }

            if (String.IsNullOrEmpty(frmMainForm.CurrentQun))
            {
                model.To = "@";
            }
            else
            {
                model.To = frmMainForm.CurrentQun;
            }
            if (frmMainForm.CurrentWX == null)
            {
                Log.WriteLogByDate("当前微信未登陆");
                model.From = "@";
            }
            else
            {
                model.From = frmMainForm.CurrentWX.UserName;
            }
            model.Type = 1;
            model.Time = DateTime.Now;
            StringBuilder content = new StringBuilder();
            switch (msg.CommandType)
            {
                case "上下查":
                    //判断是不是托，
                    //托自动回复
                    break;
                case "买名次":
                    content.Append("@" + msg.MsgFromName + " ");
                    content.Append(" 下单成功");
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("0", "十名");
                    dic.Add("1", "冠军");
                    dic.Add("2", "亚军");
                    dic.Add("3", "季军");
                    dic.Add("4", "四名");
                    dic.Add("5", "五名");
                    dic.Add("6", "六名");
                    dic.Add("7", "七名");
                    dic.Add("8", "八名");
                    dic.Add("9", "九名");

                    int userScore = 0;
                    if (msg.CommandOne.ExitHanZi())
                    {
                        foreach (Char item in msg.CommandTwo)
                        {
                            content.AppendFormat("<br/>冠军[{0}]{1} ", item, msg.Score);
                            userScore++;
                        }
                    }
                    else
                    {
                        foreach (Char item in msg.CommandOne)
                        {
                            foreach (Char item2 in msg.CommandTwo)
                            {
                                content.AppendFormat("<br/>{0}[{1}]{2}", dic[item.ToString()], item2, msg.Score);
                                userScore++;
                            }
                        }
                    }

                    //计算剩余积分
                    if ((contactScore.TotalScore - userScore * Convert.ToInt32(msg.Score)) < 0)
                    {
                        content.Clear();
                        content.Append("@" + msg.MsgFromName + " ");
                        content.Append("积分不足");
                        content.Append("<br/>当前积分：" + contactScore.TotalScore);
                        //msg.IsDeal = "1";
                        //msg.IsDelete = "1";
                        //msg.IsSucc = 2;
                        //List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                        //pkList2.Add(new KeyValuePair<string, object>("MsgId", msg.MsgId));
                        //data.Update<NowMsg>(msg,pkList2, "");
                    }
                    else
                    {
                        //msg.IsDeal = "1";
                        //msg.IsDelete = "1";
                        //msg.IsSucc = 1;
                        //List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                        //pkList2.Add(new KeyValuePair<string, object>("MsgId", msg.MsgId));
                        //data.Update<NowMsg>(msg, pkList2, "");
                        contactScore.TotalScore = contactScore.TotalScore - userScore * Convert.ToInt32(msg.Score);
                        List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                        pkList3.Add(new KeyValuePair<string, object>("Uuid", contactScore.Uuid));
                        data.Update<ContactScore>(contactScore, pkList3, "");
                        content.Append("<br/>当前积分：" + contactScore.TotalScore);
                    }

                    break;
                case "买名次大小单双龙虎":
                    content.Append("@" + msg.MsgFromName + " ");
                    content.Append(" 下单成功");
                    content.Append("<br/>"+msg.CommandOne+"名 "+msg.CommandTwo+" "+msg.Score);
                    if ((contactScore.TotalScore - Convert.ToInt32(msg.Score)) < 0)
                    {
                        content.Clear();
                        content.Append("@" + msg.MsgFromName + " ");
                        content.Append("积分不足");
                        content.Append("<br/>当前积分：" + contactScore.TotalScore);
                        //msg.IsDeal = "1";
                        //msg.IsDelete = "1";
                        //msg.IsSucc = 2;
                        //List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                        //pkList2.Add(new KeyValuePair<string, object>("MsgId", msg.MsgId));
                        //data.Update<NowMsg>(msg, pkList2, "");
                    }
                    else
                    {
                        //msg.IsDeal = "1";
                        //msg.IsDelete = "1";
                        //msg.IsSucc = 1;
                        //List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                        //pkList2.Add(new KeyValuePair<string, object>("MsgId", msg.MsgId));
                        //data.Update<NowMsg>(msg, pkList2, "");
                        contactScore.TotalScore = contactScore.TotalScore -Convert.ToInt32(msg.Score);
                        List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                        pkList3.Add(new KeyValuePair<string, object>("Uuid", contactScore.Uuid));
                        data.Update<ContactScore>(contactScore, pkList3, "");
                        content.Append("<br/>当前积分：" + contactScore.TotalScore);
                    }

                    
                    break;
                case "冠亚和":
                    content.Append("@" + msg.MsgFromName + " ");
                    content.Append(" 下单成功");
                    string[] commandTwos = msg.CommandTwo.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in commandTwos)
                    {
                        content.Append("<br/>" + msg.CommandOne + " " + item + " " + msg.Score);
                    }
                    
                    if ((contactScore.TotalScore - Convert.ToInt32(msg.Score)) < 0)
                    {
                        content.Clear();
                        content.Append("@" + msg.MsgFromName + " ");
                        content.Append("积分不足");
                        content.Append("<br/>当前积分：" + contactScore.TotalScore);
                        //msg.IsDeal = "1";
                        //msg.IsDelete = "1";
                        //msg.IsSucc = 2;
                        //List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                        //pkList2.Add(new KeyValuePair<string, object>("MsgId", msg.MsgId));
                        //data.Update<NowMsg>(msg, pkList2, "");
                    }
                    else
                    {
                        
                        //msg.IsDeal = "1";
                        //msg.IsDelete = "1";
                        //msg.IsSucc = 1;
                        //List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                        //pkList2.Add(new KeyValuePair<string, object>("MsgId", msg.MsgId));
                        //data.Update<NowMsg>(msg, pkList2, "");
                        contactScore.TotalScore = contactScore.TotalScore - Convert.ToInt32(msg.Score);
                        List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                        pkList3.Add(new KeyValuePair<string, object>("Uuid", contactScore.Uuid));
                        data.Update<ContactScore>(contactScore, pkList3, "");
                        content.Append("<br/>当前积分：" + contactScore.TotalScore);
                    }
                    break;

                case "取消":
                    content.Append("@" + msg.MsgFromName + " " + "暂不支持取消指令");
                    break;

                case "指令格式错误":
                    content.Append("@" + msg.MsgFromName + " " + "指令格式错误");

                    
                    break;

                default:
                    content.Append("@" + msg.MsgFromName + " " + "暂不支持此指令");
                    break;
            }

            msg.IsDeal = "1";
            msg.IsDelete = "1";
            msg.IsSucc = 1;
            List<KeyValuePair<string, object>> pkList4 = new List<KeyValuePair<string, object>>();
            pkList4.Add(new KeyValuePair<string, object>("MsgId", msg.MsgId));
            data.Update<NowMsg>(msg, pkList4, "");

            model.Msg = content.ToString();
            return model;
        }
    }
}
