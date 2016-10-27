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

            if (msg.CommandType != "上下查")
            {
                if (frmMainForm.IsFengPan)
                {
                    data.ExecuteSql(" delete from nowmsg where msgid=" + msg.MsgId);
                    model.Msg = "正在封盘";
                    return model;
                }
                else if (frmMainForm.IsKaiJian)
                {
                    data.ExecuteSql(" delete from nowmsg where msgid=" + msg.MsgId);
                    model.Msg = "正在开奖";
                    return model;
                }
            }

            switch (msg.CommandType)
            {
                case "上下查":
                    //判断是不是托，
                    //托自动回复
                    break;
                case "买名次":
                    content.Append("@" + msg.MsgFromName + " ");
                    content.Append(" 下注成功");
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
                            content.AppendFormat("\r\n冠军[{0}]{1} ", item, msg.Score);
                            userScore++;
                        }
                    }
                    else
                    {
                        foreach (Char item in msg.CommandOne)
                        {
                            foreach (Char item2 in msg.CommandTwo)
                            {
                                content.AppendFormat("\r\n{0}[{1}]{2}", dic[item.ToString()], item2, msg.Score);
                                userScore++;
                            }
                        }
                    }

                    //计算剩余积分
                    if ((contactScore.TotalScore - userScore * msg.Score.ToInt()) < 0)
                    {
                        content.Clear();
                        content.Append("@" + msg.MsgFromName + " ");
                        content.Append("积分不足");
                        content.Append("\r\n当前积分：" + contactScore.TotalScore);

                        //删除指令
                        data.ExecuteSql(" delete from nowmsg where MsgId=" + msg.MsgId);
                    }
                    else
                    {
                        contactScore.TotalScore = contactScore.TotalScore - userScore * msg.Score.ToInt();
                        contactScore.RunScore = contactScore.RunScore + userScore * msg.Score.ToInt();
                        List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                        pkList3.Add(new KeyValuePair<string, object>("Uuid", contactScore.Uuid));
                        data.Update<ContactScore>(contactScore, pkList3, "");
                        content.Append("\r\n当前积分：" + contactScore.TotalScore);
                    }

                    break;
                case "名次大小单双龙虎":
                    content.Append("@" + msg.MsgFromName + " ");
                    content.Append(" 下注成功");
                    Dictionary<string, string> dicMC = new Dictionary<string, string>();
                    dicMC.Add("0", "十名");
                    dicMC.Add("1", "冠军");
                    dicMC.Add("2", "亚军");
                    dicMC.Add("3", "季军");
                    dicMC.Add("4", "四名");
                    dicMC.Add("5", "五名");
                    dicMC.Add("6", "六名");
                    dicMC.Add("7", "七名");
                    dicMC.Add("8", "八名");
                    dicMC.Add("9", "九名");
                    int n2 = 0;
                    foreach (char item in msg.CommandOne)
                    {
                        if (dicMC.ContainsKey(item.ToString()))
                        {
                            content.AppendFormat("\r\n{0} {1} {2}", dicMC[item.ToString()], msg.CommandTwo, msg.Score);
                            n2++;
                        }
                    }
                    if ((contactScore.TotalScore - n2*msg.Score.ToInt()) < 0)
                    {
                        content.Clear();
                        content.Append("@" + msg.MsgFromName + " ");
                        content.Append("积分不足");
                        content.Append("\r\n当前积分：" + contactScore.TotalScore);
                        //删除指令
                        data.ExecuteSql(" delete from nowmsg where MsgId=" + msg.MsgId);
                    }
                    else
                    {
                        contactScore.TotalScore = contactScore.TotalScore - n2*msg.Score.ToInt();
                        contactScore.RunScore = contactScore.RunScore + n2*msg.Score.ToInt();
                        List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                        pkList3.Add(new KeyValuePair<string, object>("Uuid", contactScore.Uuid));
                        data.Update<ContactScore>(contactScore, pkList3, "");
                        content.Append("\r\n当前积分：" + contactScore.TotalScore);
                    }


                    break;
                case "冠亚和":
                    content.Append("@" + msg.MsgFromName + " ");
                    if (string.IsNullOrEmpty(msg.CommandTwo))
                    {
                        content.Append("指令格式错误");
                        data.ExecuteSql(" delete from nowmsg where MsgId=" + msg.MsgId);
                        break;
                    }

                    int n = 0;

                    string comTwo = msg.CommandTwo.Replace("/", "");
                    if (comTwo.Length<=11&&Convert.ToInt32(comTwo) <= 19)
                    {
                        switch (comTwo)
                        {
                            case "3":
                            case "4":
                            case "5":
                            case "6":
                            case "7":
                            case "8":
                            case "9":
                            case "10":
                            case "11":
                            case "12":
                            case "13":
                            case "14":
                            case "15":
                            case "16":
                            case "17":
                            case "18":
                            case "19":
                                n++;
                                break;
                        }
                    }
                    else
                    {
                        //多条和指令
                        while (comTwo.Length > 0)

                        {
                            char first = comTwo.First();

                            if (first.ToString().ToInt() >= 3 && first.ToString().ToInt() <= 9)
                            {
                                switch (first.ToString())
                                {
                                    case "3":
                                    case "4":
                                    case "5":
                                    case "6":
                                    case "7":
                                    case "8":
                                    case "9":
                                        n++;
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
                                        case "11":
                                        case "12":
                                        case "13":
                                        case "14":
                                        case "15":
                                        case "16":
                                        case "17":
                                        case "18":
                                        case "19":
                                            n++;
                                            break;
                                    }

                                    comTwo = string.Join("", comTwo.Reverse());
                                    comTwo = comTwo.Remove(0, 2);
                                    comTwo = string.Join("", comTwo.Reverse());
                                }
                            }
                        }
                    }
                    content.Append(" 下注成功");
                    string[] commandTwos = msg.CommandTwo.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in commandTwos)
                    {
                        content.Append("\r\n" + msg.CommandOne + " " + item + " " + msg.Score);
                    }

                    if ((contactScore.TotalScore - n * msg.Score.ToInt()) < 0)
                    {
                        content.Clear();
                        content.Append(msg.MsgFromName + " ");
                        content.Append("积分不足");
                        content.Append("\r\n当前积分：" + contactScore.TotalScore);

                        //删除指令
                        data.ExecuteSql(" delete from nowmsg where MsgId=" + msg.MsgId);
                    }
                    else
                    {
                        contactScore.TotalScore = contactScore.TotalScore - n * msg.Score.ToInt();
                        contactScore.RunScore = contactScore.RunScore + n * msg.Score.ToInt();
                        List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                        pkList3.Add(new KeyValuePair<string, object>("Uuid", contactScore.Uuid));
                        data.Update<ContactScore>(contactScore, pkList3, "");
                        content.Append("\r\n当前积分：" + contactScore.TotalScore);
                    }
                    break;
                case "和大":
                case "和小":
                case "和单":
                case "和双":
                    content.Append("@" + msg.MsgFromName + " " + "下注成功");
                    content.Append("\r\n" + msg.CommandType + " " + msg.Score);
                    contactScore.TotalScore = contactScore.TotalScore - msg.Score.ToInt();
                    contactScore.RunScore = contactScore.RunScore + msg.Score.ToInt();

                    List<KeyValuePair<string, object>> pkList5 = new List<KeyValuePair<string, object>>();
                    pkList5.Add(new KeyValuePair<string, object>("Uuid", contactScore.Uuid));
                    data.Update<ContactScore>(contactScore, pkList5, "");
                    content.Append("\r\n当前积分：" + contactScore.TotalScore);

                    break;

                case "取消":
                case "取消指令":
                    content.Append("@" + msg.MsgFromName + " " + "取消下注成功");
                    data.ExecuteSql("delete from Nowmsg  where msgfromid=" + msg.MsgFromId + " and period=" + msg.Period);
                    data.ExecuteSql(string.Format("update contactscore set totalscore=totalscore+runscore where uin={0}", msg.MsgFromId));
                    data.ExecuteSql(string.Format("update contactscore set runscore=0 where uin={0}", msg.MsgFromId));
                    break;

                case "指令格式错误":
                    content.Append("@" + msg.MsgFromName + " " + "指令格式错误");
                    //删除指令
                    data.ExecuteSql(" delete from nowmsg where MsgId=" + msg.MsgId);
                    break;
                case "下注积分范围错误":
                    content.Append("@" + msg.MsgFromName + " " + "下注积分范围错误");
                    //删除指令
                    data.ExecuteSql(" delete from nowmsg where MsgId=" + msg.MsgId);
                    break;
                default:
                    content.Append("@" + msg.MsgFromName + " " + "暂不支持此指令");
                    //删除指令
                    data.ExecuteSql(" delete from nowmsg where MsgId=" + msg.MsgId);
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
