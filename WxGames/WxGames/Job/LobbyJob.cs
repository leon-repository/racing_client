using BLL;
using Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            Lobby.Instanc.Start();
        }
    }
}
