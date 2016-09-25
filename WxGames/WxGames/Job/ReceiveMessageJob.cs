using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using WxGames;
using Newtonsoft.Json.Linq;
using System.IO;

namespace WxGames.Job
{
    class ReceiveMessageJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //获取消息列表，并原样输出
            string sync_flag=frmMainForm.wxs.WxSyncCheck();//同步检查
            if (sync_flag == null)
            {
                return;
            }

            JObject sync_result = frmMainForm.wxs.WxSync();//进行同步
            if (sync_result == null)
            {
                return;
            }

            if (sync_result["AddMsgCount"] != null && sync_result["AddMsgCount"].ToString() != "0")
            {
                foreach (JObject m in sync_result["AddMsgList"])
                {
                    string from = m["FromUserName"].ToString();
                    string to = m["ToUserName"].ToString();
                    string content = m["Content"].ToString();
                    string type = m["MsgType"].ToString();
                    //string uin = m["Uin"].ToString();
                    //string nickName = m["NickName"].ToString();

                    StringBuilder msgContext = new StringBuilder(string.Format("@{0} 你的uid是{1},发送的内容是{0}", "", "", content));
                    File.AppendAllText("E:" + DateTime.Now.ToString("yyyy-MM-dd") + "E.txt", DateTime.Now.ToString() + "同步的消息：" + msgContext);

                    if (from == frmMainForm.CurrentQun)
                    {
                        WXMsg msg = new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = msgContext.ToString(), To = frmMainForm.CurrentQun, Type = 1, Readed = false, Time = DateTime.Now };
                        frmMainForm.CurrentWX.SendMsg(msg, false);
                    }

                }
            }

        }
    }
}
