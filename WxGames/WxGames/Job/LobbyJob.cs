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
    [DisallowConcurrentExecution]
    public class LobbyJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Lobby.Instanc.Start();
            }
            catch (Exception ex)
            {
                Log.WriteLogByDate("获取开奖信息JOB失败，原因是：");
                Log.WriteLog(ex);
            }
        }
    }
}
