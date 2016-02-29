using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPWAcitveDingShi
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region 定时发送给,注册用户：提示邮箱激活
            ISchedulerFactory sf1 = new StdSchedulerFactory();  //创建一个计划工厂

            IScheduler sched1 = sf1.GetScheduler();             //创建一个计划着

            JobDetail job1 = new JobDetail("job2",              //任务名
                                          "group2",             //任务组
                               typeof(SendActiveUserEmail)   //执行哪个Job,、执行任务类,实现了IJob接口的类   
                                          );

            DateTime ts1 = TriggerUtils.GetNextGivenSecondDate(null, 1);//1秒后开始
            TimeSpan interval1 = TimeSpan.FromSeconds(2);//开始间隔2秒，依次发送   //创建定时

            //每若干小时运行一次，小时间隔由appsettings中的IndexIntervalHour参数指定
            Trigger trigger1 = new SimpleTrigger("trigger2",   //触发器名
                                                    "group2",  //触发器组
                                                    "job2",
                                                    "group2",
                                                     ts1,
                                                     null,
                                                    SimpleTrigger.RepeatIndefinitely,
                                                    interval1);

            //相比，Timer：，Quartz ,更加精准。
            sched1.AddJob(job1, true);
            sched1.ScheduleJob(trigger1);
            sched1.Start();
            Console.WriteLine("激活邮件定时服务器启动...");
            #endregion
        }
    }
}
