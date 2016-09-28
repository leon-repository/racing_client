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

namespace WxGames.Job
{
    /// <summary>
    /// 接受群消息，并保存到数据库
    /// </summary>
    class ReceiveMessageJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ////获取消息列表，并原样输出
            //string sync_flag=frmMainForm.wxs.WxSyncCheck();//同步检查
            //if (sync_flag == null)
            //{
            //    return;
            //}

            //JObject sync_result = frmMainForm.wxs.WxSync();//进行同步
            //if (sync_result == null)
            //{
            //    return;
            //}

            //SystemContext systemContext = new SystemContext();
            ////保存微信群联系人
            ////1，获取群信息
            ////只有人员有变动，才会有值，才能获取到uin
            //JToken contactList = sync_result["ModContactList"];
            //string qUserName = "";
            //string qNickName = "";
            //string qChatRoomOwner = "";
            //if (contactList != null)
            //{
            //    foreach (JObject qun in contactList)
            //    {
            //        if (qun["UserName"].ToString() != frmMainForm.CurrentQun)
            //        {
            //            continue;
            //        }
            //        qUserName = qun["UserName"].ToString();
            //        qNickName = qun["NickName"].ToString();
            //        qChatRoomOwner = qun["ChatRoomOwner"].ToString();

            //        JToken memberList = qun["MemberList"];
            //        foreach (JObject member in memberList)
            //        {
            //            string uin = member["Uin"].ToString();
            //            string userName = member["UserName"].ToString();
            //            string nickName = member["NickName"].ToString();
            //            string pyQuanPin = member["PYQuanPin"].ToString();
            //            string remark = member["RemarkPYQuanPin"].ToString();

            //            Contact modelOne=systemContext.Contacts.FirstOrDefault(p => p.Uin == uin);
            //            if (modelOne == null)
            //            {
            //                Contact model = new Contact();
            //                model.Uuid = Guid.NewGuid().ToString();
            //                model.NickName = nickName;
            //                model.Uin = uin;
            //                model.UserName = userName;
            //                model.QuanPin = pyQuanPin;
            //                model.QuserName = qUserName;
            //                model.QnickName = qNickName;
            //                model.Remark = remark;
            //                systemContext.Contacts.Add(model);
            //                systemContext.SaveChanges();
            //            }
            //            else
            //            {
            //                modelOne.NickName = nickName;
            //                modelOne.UserName = userName;
            //                modelOne.QuanPin = pyQuanPin;
            //                modelOne.QuserName = qUserName;
            //                modelOne.QnickName = qNickName;
            //                modelOne.Remark = remark;
            //                systemContext.Contacts.Attach(modelOne);
            //                systemContext.SaveChanges();
            //            }
            //        }
            //    }
            //}

            ////获取群里所有联系人，无uuin，根据昵称来更新userName和uin
            //string qun1 = frmMainForm.wxs.GetQun(frmMainForm.CurrentQun);
            //JObject qunObj=JsonConvert.DeserializeObject(qun1) as JObject;
            //if (qunObj["MemberCount"] != null && qunObj["MemberCount"].ToString() != "0")
            //{
            //    foreach (JObject member in qunObj["MemberList"])
            //    {
            //        Contact modelOne = systemContext.Contacts.FirstOrDefault(p => p.NickName==member["NickName"].ToString());
            //        modelOne.UserName = member["UserName"].ToString();
            //        systemContext.Contacts.Attach(modelOne);
            //        systemContext.SaveChanges();
            //    }
            //}


            ////获取到消息
            //if (sync_result["AddMsgCount"] != null && sync_result["AddMsgCount"].ToString() != "0")
            //{
            //    foreach (JObject m in sync_result["AddMsgList"])
            //    {
            //        if (m["FromUserName"].ToString() != frmMainForm.CurrentQun)
            //        {
            //            continue;
            //        }

            //        string[] content= m["Content"].ToString().Split(new string[] { "<br/>"}, StringSplitOptions.RemoveEmptyEntries); 

            //        OriginMsg msg = new OriginMsg();
            //        msg.MsgId = m["MsgId"].ToString();
            //        msg.FromUserName = content[0];
            //        msg.ToUserName = m["ToUserName"].ToString();
            //        msg.MsgType = m["MsgType"].ToString();
            //        msg.Content = m["Content"].ToString();
            //        msg.Status = m["Status"].ToString();
            //        msg.ImgStatus = m["ImgStatus"].ToString();
            //        msg.CreateTime = m["CreateTime"].ToString();
            //        msg.NewMsgId = m["NewMsgId"].ToString();
            //        msg.IsSucc = "0";
            //        Contact model = systemContext.Contacts.FirstOrDefault<Contact>(p => p.UserName == msg.FromUserName);
            //        if (model != null)
            //        {
            //            msg.FromNickName = model.NickName;
            //            msg.FromUin = model.Uin;
            //            msg.QnickName = qNickName;
            //        }
            //        systemContext.OriginMsgs.Add(msg);
            //        systemContext.SaveChanges();
            //    }
            //}
        }
    }
}
