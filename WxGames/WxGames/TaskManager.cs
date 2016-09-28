using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz.Impl;
using Quartz;
using WxGames.Job;

namespace WxGames
{
    /// <summary>
    /// 任务管理
    /// </summary>
    public class TaskManager
    {
        private TaskManager() {
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            //发送消息
            IJobDetail job = JobBuilder.Create<SendMessageJob>()
                .WithIdentity("sendMessage", "job").Build();

            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("sendMessageTri", "Trigger")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).WithRepeatCount(3))
                .ForJob(job)
                .Build();

            //接收消息
            IJobDetail job2 = JobBuilder.Create<ReceiveMessageJob>()
                .WithIdentity("receiveMessage", "job").Build();

            ISimpleTrigger trigger2 = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("receiveMessageTri", "Trigger")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(3).WithRepeatCount(int.MaxValue))
                .ForJob(job2)
                .Build();

            //把job，trigger添加到任务中
            sched.ScheduleJob(job, trigger);
            sched.ScheduleJob(job2, trigger2);

            Scheduler = sched;
        }

        //发送消息任务
        public IScheduler Scheduler = null;

        public static readonly TaskManager Instance = new TaskManager();

        /// <summary>
        /// 开始任务
        /// <param name="status">开始true,结束false</param>
        /// </summary>
        public void Start(bool status)
        {
            if (status)
            {
                if (Scheduler.IsStarted)
                {
                    Scheduler.ResumeAll();
                }
                Scheduler.Start();

            }
            else
            {
                Scheduler.PauseAll();
            }

        }
    }
}
