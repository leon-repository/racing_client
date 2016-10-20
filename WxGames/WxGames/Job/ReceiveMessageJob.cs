using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using WxGames;
using Newtonsoft.Json.Linq;
using System.IO;
using Model;
using BLL;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;

namespace WxGames.Job
{
    /// <summary>
    /// 接受群消息，并保存到数据库
    /// </summary>
    class ReceiveMessageJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            NewMethod();
            sw.Stop();

            //Log.WriteLogByDate("接受群消息使用时间："+sw.ElapsedTicks);
        }

        public static void NewMethod()
        {
            //Log.WriteLogByDate("开始同步检查");
            ////获取消息列表，并原样输出
            //string sync_flag = frmMainForm.wxs.WxSyncCheck();//同步检查

            //if (sync_flag.Contains("11"))
            //{
            //    return;
            //}

            //if (sync_flag == null)
            //{
            //    return;
            //}
            //Log.WriteLogByDate("结束同步检查");

            Log.WriteLogByDate("开始消息同步检查");
            JObject sync_result = frmMainForm.wxs.WxSync();//进行同步
            if (sync_result == null)
            {
                return;
            }
            Log.WriteLogByDate("结束消息同步检查");
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
                    qNickName = qun["NickName"].ToString();
                    qChatRoomOwner = qun["ChatRoomOwner"].ToString();

                    JToken memberList = qun["MemberList"];
                    foreach (JObject member in memberList)
                    {
                        string uin = member["Uin"].ToString();
                        string userName = member["UserName"].ToString();
                        string nickName = member["NickName"].ToString();
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
                    }
                }
            }

            //获取群里所有联系人，无uuin，根据昵称来更新userName和uin
            string qun1 = frmMainForm.wxs.GetQun(frmMainForm.CurrentQun);
            Log.WriteLogByDate("群json:"+qun1);
            JObject qunObj = JsonConvert.DeserializeObject(qun1) as JObject;

            if (qunObj["MemberList"] != null)
            {
                foreach (JObject member in qunObj["MemberList"])
                {
                    List<KeyValuePair<string, object>> pkList2 = new List<KeyValuePair<string, object>>();
                    pkList2.Add(new KeyValuePair<string, object>("NickName", member["NickName"].ToString()));
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


            //获取到消息
            if (sync_result["AddMsgCount"] != null && sync_result["AddMsgCount"].ToString() != "0")
            {
                foreach (JObject m in sync_result["AddMsgList"])
                {
                    if (m["FromUserName"].ToString() != frmMainForm.CurrentQun)
                    {
                        continue;
                    }

                    string[] content = m["Content"].ToString().Split(new string[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                    if (content != null && content.Length == 2)
                    {
                        OriginMsg msg = new OriginMsg();
                        msg.MsgId = m["MsgId"].ToString();
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
                                data.Insert<OriginMsg>(msg, "");
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
