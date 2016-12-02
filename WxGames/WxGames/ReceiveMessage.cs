using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WxGames;
using Newtonsoft.Json.Linq;
using System.IO;
using Model;
using BLL;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;

namespace WxGames
{
    public class ReceiveMessage
    {
        public static readonly ReceiveMessage Instance = new ReceiveMessage();

        public List<string> OrderKey = new List<string>() { "大","小","单","双","龙","虎","和","0","1","2","3","4","5","6","7","8","9","取消"};

        public void Start()
        {
            try
            {
                Log.WriteLogByDate("接受消息调用开始");
                NewMethod();
                Log.WriteLogByDate("接受消息调用结束");
            }
            catch (Exception ex)
            {
                Log.WriteLogByDate("接收群消息，保存到数据库功能失败，原因是:");
                Log.WriteLog(ex);
            }
        }

        public void NewMethod()
        {
            JObject sync_result = frmMainForm.wxs.WxSync();//进行同步

            if (sync_result == null)
            {
                return;
            }
            string conn = ConfigurationManager.AppSettings["conn"].ToString();
            DataHelper data = new DataHelper(conn);
            //保存微信群联系人
            //1，获取群信息
            //只有人员有变动，才会有值，才能获取到uin
            JToken contactList = sync_result["ModContactList"];
            string qUserName = "";
            string qNickName = "";
            string qChatRoomOwner = "";
            if (contactList != null)
            {
                foreach (JObject qun in contactList)
                {
                    if (qun["UserName"].ToString() != frmMainForm.CurrentQun)
                    {
                        continue;
                    }
                    qUserName = qun["UserName"].ToString();
                    qNickName = qun["NickName"].ToString().ClearHtml();
                    qChatRoomOwner = qun["ChatRoomOwner"].ToString();

                    JToken memberList = qun["MemberList"];
                    foreach (JObject member in memberList)
                    {
                        string uin = member["Uin"].ToString();
                        string userName = member["UserName"].ToString();
                        string nickName = member["NickName"].ToString().ClearHtml();
                        string pyQuanPin = member["PYQuanPin"].ToString();
                        string remark = member["RemarkPYQuanPin"].ToString();

                        List<KeyValuePair<string, object>> pkList = new List<KeyValuePair<string, object>>();
                        pkList.Add(new KeyValuePair<string, object>("Uin", uin));
                        Contact modelOrgin = data.First<Contact>(pkList, "");
                        if (modelOrgin == null)
                        {
                            Contact model = new Contact();
                            model.Uuid = Guid.NewGuid().ToString();
                            model.NickName = nickName;
                            model.Uin = uin;
                            model.UserName = userName;
                            model.QuanPin = pyQuanPin;
                            model.QuserName = qUserName;
                            model.QnickName = qNickName;
                            model.Remark = remark;
                            data.Insert<Contact>(model, "");
                        }
                        else
                        {
                            modelOrgin.NickName = nickName;
                            modelOrgin.UserName = userName;
                            modelOrgin.QuanPin = pyQuanPin;
                            modelOrgin.QuserName = qUserName;
                            modelOrgin.QnickName = qNickName;
                            modelOrgin.Remark = remark;
                            data.Update<Contact>(modelOrgin, pkList, "");
                        }

                        //根据userName更新nickName
                        List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                        pkList2.Add(new KeyValuePair<string, object>("UserName", userName));
                        Contact modelOrgin2 = data.First<Contact>(pkList2, "");
                        if (modelOrgin2 != null)
                        {
                            modelOrgin2.NickName = nickName;
                            data.Update<Contact>(modelOrgin2, pkList2, "");
                        }
                    }
                }
            }

            //获取群里所有联系人，无uuin，根据昵称来更新userName和uin
            string qun1 = frmMainForm.wxs.GetQun(frmMainForm.CurrentQun);
            //Log.WriteLogByDate("群成员列表json:"+qun1);

            if (!string.IsNullOrEmpty(qun1))
            {
                JObject qunObj = JsonConvert.DeserializeObject(qun1) as JObject;

                if (qunObj["MemberList"] != null)
                {
                    foreach (JObject member in qunObj["MemberList"])
                    {
                        List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                        pkList2.Add(new KeyValuePair<string, object>("NickName", member["NickName"].ToString().ClearHtml()));
                        Contact modelOne = data.First<Contact>(pkList2, "");
                        if (modelOne != null)
                        {
                            modelOne.UserName = member["UserName"].ToString();
                            List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                            pkList3.Add(new KeyValuePair<string, object>("Uin", modelOne.Uin));
                            data.Update<Contact>(modelOne, pkList3, "");
                        }
                    }
                }
            }

            Log.WriteLogByDate("开始更新本地昵称：qunObj[\"ContactList\"][0][\"MemberList\"]");
            if (!string.IsNullOrEmpty(qun1))//更新本地昵称
            {
                JObject qunObj = JsonConvert.DeserializeObject(qun1) as JObject;

                //if (qunObj["MemberList"] != null)
                //{
                //    foreach (JObject member in qunObj["MemberList"])
                //    {
                //        List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                //        pkList2.Add(new KeyValuePair<string, object>("UserName", member["UserName"].ToString()));
                //        Contact modelOne = data.First<Contact>(pkList2, "");
                //        if (modelOne != null)
                //        {
                //            modelOne.NickName = member["NickName"].ToString().ClearHtml();
                //            List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                //            pkList3.Add(new KeyValuePair<string, object>("UserName", modelOne.UserName));
                //            data.Update<Contact>(modelOne, pkList3, "");
                //        }
                //    }
                //}
                if (qunObj["ContactList"][0]["MemberList"] != null)
                {
                    foreach (JObject member in qunObj["ContactList"][0]["MemberList"])
                    {
                        List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                        pkList2.Add(new KeyValuePair<string, object>("UserName", member["UserName"].ToString()));
                        Contact modelOne = data.First<Contact>(pkList2, "");
                        if (modelOne != null)
                        {
                            modelOne.NickName = member["NickName"].ToString().ClearHtml();
                            List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                            pkList3.Add(new KeyValuePair<string, object>("UUID", modelOne.Uuid));
                            data.Update<Contact>(modelOne, pkList3, "");

                            //更新contactScore的昵称
                            List<KeyValuePair<string, object>> pkList4 = new List<KeyValuePair<string, object>>();
                            pkList4.Add(new KeyValuePair<string, object>("UIN", modelOne.Uin));
                            ContactScore score = data.First<ContactScore>(pkList4, "");
                            //score.NickName = modelOne.NickName;
                            //List<KeyValuePair<string, object>> pkList5 = new List<KeyValuePair<string, object>>();
                            //pkList5.Add(new KeyValuePair<string, object>("UUID", score.Uuid));
                            //data.Update<ContactScore>(score, pkList5, "");
                            if (score != null)
                            {
                                data.ExecuteSql(string.Format("update contactScore set nickname='{0}' where uuid='{1}'", modelOne.NickName, score.Uuid));
                            }
                         }
                    }
                }
            }

            Log.WriteLogByDate("开始保存消息");
            //获取到消息
            if (sync_result["AddMsgCount"] != null && sync_result["AddMsgCount"].ToString() != "0")
            {
               // Log.WriteLogByDate("获取消息：");

                foreach (JObject m in sync_result["AddMsgList"])
                {
                    if (m["FromUserName"].ToString() != frmMainForm.CurrentQun)
                    {
                        continue;
                    }

                    string[] content = m["Content"].ToString().Split(new string[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                    if (content != null && content.Length == 2)
                    {
                        //Log.WriteLogByDate("获取消息："+ content[1]);

                        OriginMsg msg = new OriginMsg();
                        msg.MsgId = m["MsgId"].ToString()+ m["CreateTime"].ToString();//用msgId和createTime
                        string userName = content[0].Replace(":", "");
                        msg.FromUserName = userName;
                        msg.ToUserName = m["ToUserName"].ToString();
                        msg.MsgType = m["MsgType"].ToString();
                        msg.Content = content[1];
                        msg.Status = m["Status"].ToString();
                        msg.ImgStatus = m["ImgStatus"].ToString();
                        msg.CreateTime = m["CreateTime"].ToString();
                        msg.NewMsgId = m["NewMsgId"].ToString();
                        msg.IsSucc = "0";
                        List<KeyValuePair<string, object>> pkList3 = new List<KeyValuePair<string, object>>();
                        pkList3.Add(new KeyValuePair<string, object>("UserName", userName));
                        Contact model3 = data.First<Contact>(pkList3, "");
                        if (model3 != null)
                        {
                            msg.FromNickName = model3.NickName;
                            msg.FromUin = model3.Uin;
                            msg.QnickName = qNickName;
                            //插入消息前检查
                            List<KeyValuePair<string, object>> pkList4 = new List<KeyValuePair<string, object>>();
                            pkList4.Add(new KeyValuePair<string, object>("MsgId", msg.MsgId));

                            OriginMsg orginMsg2 = data.First<OriginMsg>(pkList4, "");
                            if (orginMsg2 == null)
                            {
                                if(msg.FromUin!="0")
                                {
                                    //简单检查指令
                                    if (OrderFirstCheck.Instance.CheckOne(msg.Content))
                                    {
                                        data.Insert<OriginMsg>(msg, "");
                                    }
                                    else
                                    {
                                        //检查是否封盘
                                        //获取开奖信息，并将开奖信息保存到数据库

                                        if (frmMainForm.IsComplete && frmMainForm.IsJieDan)
                                        {
                                            //简单检查指令是否存在关键字，
                                            foreach (string item in OrderKey)
                                            {
                                                if (msg.Content.Contains(item))
                                                {
                                                    data.Insert<OriginMsg>(msg, "");
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //封盘阶段
                                            WXMsg model = new WXMsg() { To = frmMainForm.CurrentQun, From = frmMainForm.CurrentWX.UserName, Msg = "正在封盘" };
                                            frmMainForm.CurrentWX.SendMsg(model, false);
                                            continue;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                Log.WriteLogByDate("OriginMsg消息重复：msgID=" + msg.MsgId);
                            }
                        }
                        else
                        {
                            Log.WriteLogByDate("无法获取到uin，读取到的消息不保存");
                        }

                    }
                }
            }
        }
    }
}
