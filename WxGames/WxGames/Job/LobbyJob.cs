using BLL;
using Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WxGames.Job
{
    /// <summary>
    /// 获取开奖信息，并发送开奖过程中的消息
    /// </summary>
    public class LobbyJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            NewMethod();
            sw.Stop();

            Log.WriteLogByDate("获取开奖时间：" + sw.ElapsedTicks);
        }

        private void NewMethod()
        {
            Lobby.Instanc.Start();
        }
    }
}
