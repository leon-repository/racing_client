using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using WxGames;
using BLL;
using Model;
using System.Configuration;
using System.Diagnostics;

namespace WxGames.Job
{
    [DisallowConcurrentExecution]
    public class SendMessageJob : IJob
    {
        public string JObject { get; private set; }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                NewMethod();
            }
            catch (Exception ex)
            {
                Log.WriteLogByDate("发送消息JOB失败，原因是:");
                Log.WriteLog(ex);
            }
        }

        private static void NewMethod()
        {
            string conn = ConfigurationManager.AppSettings["conn"].ToString();
            DataHelper data = new DataHelper(conn);
            //处理数据
            List<OriginMsg> orginList = new List<OriginMsg>();
            orginList = data.GetList<OriginMsg>(" issucc='0' ", "");

            foreach (OriginMsg msg in orginList)
            {
                Order order = OrderManager.Instance.ToOrder(msg.Content);
                NowMsg nowMsg = new NowMsg();
                nowMsg.MsgId = msg.MsgId.ToString();
                nowMsg.MsgFromId = msg.FromUin;
                nowMsg.MsgFromName = msg.FromNickName;
                nowMsg.ReciveId = msg.ToUserName;//接受人均为群
                nowMsg.ReciveName = msg.ToUserName;
                nowMsg.MsgContent = "";//未赋值
                nowMsg.OrderContect = msg.Content;
                nowMsg.CommandOne = order.CommandOne;
                nowMsg.CommandTwo = order.CommandTwo;
                nowMsg.Score = order.Score;
                nowMsg.CommandType = order.CommandType.ToString();
                nowMsg.CreateDate = msg.CreateTime;
                nowMsg.OpDate = DateTime.Now.ToString();
                nowMsg.IsSucc = 0;
                nowMsg.IsDelete = "0";
                nowMsg.IsMsg = order.CommandType.ToString();
                nowMsg.IsDeal = "0";
                nowMsg.Period = frmMainForm.Perioid;

                //先查消息id是否重复，如果重复，则跳过不处理
                List<KeyValuePair<string, object>> pkList = new List<KeyValuePair<string, object>>();
                pkList.Add(new KeyValuePair<string, object>("MsgId", msg.MsgId));
                NowMsg exitNowMsg = data.First<NowMsg>(pkList, "");
                if (exitNowMsg == null)
                {
                    data.Insert<NowMsg>(nowMsg, "");
                }
                else
                {
                    Log.WriteLogByDate("NowMsg消息重复：msgID=" + msg.MsgId);
                }
                data.ExecuteSql(string.Format("update OriginMsg set IsSucc = '1' where MsgId = '{0}'", msg.MsgId));
            }


            //处理指令，（下注指令要回复，错误指令要回复）
            List<NowMsg> nowMsgList = data.GetList<NowMsg>(" CommandType not in ('上下查') and isdelete='0' ", "");
            foreach (NowMsg msg in nowMsgList)
            {
                //1,处理指令
                WXMsg model = Msg2WxMsg.Instance.GetMsg(msg);

                //2,判断是否发送消息（如果返回的消息不为空）
                //成功提示，错误格式提示，余额不足提示，投注限制提示，封盘提示，
                if (model != null)
                {
                    frmMainForm.CurrentWX.SendMsg(model, false);
                }
            }
        }
    }
}
