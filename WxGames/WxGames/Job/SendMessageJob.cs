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

namespace WxGames.Job
{
    public class SendMessageJob : IJob
    {
        public string JObject { get; private set; }

        public void Execute(IJobExecutionContext context)
        {
            //处理数据
            SystemContext systemContext = new SystemContext();
            IQueryable<OriginMsg> orginList = null;
                orginList = systemContext.OriginMsgs.Where(p => p.IsSucc == "0");

            foreach (OriginMsg msg in orginList)
            {
                //内容切割一下
                string[] contents = msg.Content.Split(new string[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries);
                if (contents == null)
                {
                    continue;
                }
                if (contents.Length == 2)
                {
                    Order order = OrderManager.Instance.ToOrder(contents[1]);
                    NowMsg nowMsg = new NowMsg();
                    nowMsg.MsgId = msg.MsgId.ToString();
                    nowMsg.MsgFromId = msg.FromUin;
                    nowMsg.MsgFromName = msg.FromNickName;
                    nowMsg.ReciveId = msg.ToUserName;//接受人均为群
                    nowMsg.ReciveName = msg.ToUserName;
                    nowMsg.MsgContent = "";//未赋值
                    nowMsg.OrderContect = contents[1];
                    nowMsg.CommandOne = order.CommandOne;
                    nowMsg.CommandTwo = order.CommandTwo;
                    nowMsg.Socre = order.Score;
                    nowMsg.CommandType = order.CommandType.ToString();
                    nowMsg.CreateDate = msg.CreateTime;
                    nowMsg.OpDate = DateTime.Now.ToString();
                    nowMsg.IsSucc = 0;
                    nowMsg.IsDelete = "0";
                    nowMsg.IsMsg = order.CommandType.ToString();
                    nowMsg.IsDeal = "0";
                    nowMsg.Period = frmMainForm.Perioid;
                    systemContext.Database.ExecuteSqlCommand(string.Format("insert into NowMsg  (msgId,msgFromId,msgFromName,ReciveId,ReciveName,MsgContent,OrderContect,CommandOne,CommandTwo,Score,CommandType,CreateDate,OpDate,IsSucc,IsDelete,IsMsg,IsDeal,Period) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')", msg.MsgId.ToString(), msg.FromUin, msg.FromNickName, msg.ToUserName, msg.ToUserName, "", contents[1], order.CommandOne, order.CommandTwo, order.Score, order.CommandType.ToString(), msg.CreateTime, DateTime.Now.ToString(), 0, "0", order.CommandType.ToString(), "0", frmMainForm.Perioid));
                    systemContext.Database.ExecuteSqlCommand(string.Format("update OriginMsg set IsSucc='1' where MsgId='{0}'",msg.MsgId));

                    systemContext.SaveChanges();
                }
            }
            
            //处理，指令，错误指令，查分指令

            ////发送消息
            //WXMsg msg = new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = "开始发送消息，2秒一次,无限次" + DateTime.Now.Second, To = frmMainForm.CurrentQun, Type = 3, Readed = false, Time = DateTime.Now };
            //frmMainForm.CurrentWX.SendMsg(msg, false);
        }
    }
}
