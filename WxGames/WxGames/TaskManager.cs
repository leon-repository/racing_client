using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Quartz.Impl;
using Quartz;
using WxGames.Job;
using System.Collections.Specialized;

namespace WxGames
{
    /// <summary>
    /// 任务管理
    /// </summary>
    public class TaskManager
    {
        private TaskManager()
        {
            var properties = new NameValueCollection();
            properties[""] = "微信游戏";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool,Quartz";
            properties["quartz.threadPool.threadCount"] = "100";
            properties["quartz.threadPool.threadPriority"] = "Normal";


            ISchedulerFactory sf = new StdSchedulerFactory(properties);

            IScheduler sched = sf.GetScheduler();
            //发送消息
            IJobDetail job = JobBuilder.Create<SendMessageJob>()
                .WithIdentity("sendMessage", "job").Build();

            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("sendMessageTri", "Trigger")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(1).WithRepeatCount(int.MaxValue))
                .ForJob(job)
                .Build();

            //接收消息
            IJobDetail job2 = JobBuilder.Create<ReceiveMessageJob>()
                .WithIdentity("receiveMessage", "job").Build();

            ISimpleTrigger trigger2 = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("receiveMessageTri", "Trigger")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(1).WithRepeatCount(int.MaxValue))
                .ForJob(job2)
                .Build();

            IJobDetail job3 = JobBuilder.Create<LobbyJob>()
                .WithIdentity("lobby", "job").Build();

            ISimpleTrigger trigger3 = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("lobbyTri", "Trigger")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(2).WithRepeatCount(int.MaxValue))
                .ForJob(job3)
                .Build();


            //IJobDetail job4 = JobBuilder.Create<PerformJob>()
            //   .WithIdentity("perform", "job").Build();

            //ISimpleTrigger trigger4 = (ISimpleTrigger)TriggerBuilder.Create()
            //    .WithIdentity("performTri", "Trigger")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).WithRepeatCount(int.MaxValue))
            //    .ForJob(job4)
            //    .Build();

            //把job，trigger添加到任务中
            sched.ScheduleJob(job, trigger);
            sched.ScheduleJob(job2, trigger2);
            sched.ScheduleJob(job3, trigger3);
            //sched.ScheduleJob(job4, trigger4);

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
