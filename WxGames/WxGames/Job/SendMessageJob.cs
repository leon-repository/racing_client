using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz;
using WxGames;

namespace WxGames.Job
{
    public class SendMessageJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ////处理数据

            ////发送消息
            WXMsg msg = new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = "开始发送消息，2秒一次,无限次" + DateTime.Now.Second, To = frmMainForm.CurrentQun, Type = 3, Readed = false, Time = DateTime.Now };
            frmMainForm.CurrentWX.SendMsg(msg, false);
        }
    }
}
