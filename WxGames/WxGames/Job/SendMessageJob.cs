using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using WxGames;
using BLL;
using Model;
using System.Data.Entity;
using System.Configuration;

namespace WxGames.Job
{
    public class SendMessageJob : IJob
    {
        public string JObject { get; private set; }

        public void Execute(IJobExecutionContext context)
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

                data.Insert<NowMsg>(nowMsg, "");
                data.ExecuteSql(string.Format("update OriginMsg set IsSucc = '1' where MsgId = '{0}'", msg.MsgId));
            }
        }

        //处理，指令，错误指令，查分指令

        ////发送消息
        //WXMsg msg = new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = "开始发送消息，2秒一次,无限次" + DateTime.Now.Second, To = frmMainForm.CurrentQun, Type = 3, Readed = false, Time = DateTime.Now };
        //frmMainForm.CurrentWX.SendMsg(msg, false);

    }
}
