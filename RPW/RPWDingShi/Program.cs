using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPWNewRegDingShi
{
    class Program
    {
        static void Main(string[] args)
        {

            //如果写在项目中，“定时代码”要放在Application_Start()方法中；

            #region 定时发送，给系统管理员：有新用户注册

            ISchedulerFactory sf = new StdSchedulerFactory(); //创建一个计划工厂
            IScheduler sched = sf.GetScheduler();     //创建一个计划着
            JobDetail job = new JobDetail("job1", "group1", typeof(SendNewRegUserEmailJob));//MyJog为实现了IJob接口的类
            DateTime ts = TriggerUtils.GetNextGivenSecondDate(null, 1);//1秒后开始
            TimeSpan interval = TimeSpan.FromMinutes(5);//开始间隔10秒，依次发送   //创建定时

            Trigger trigger = new SimpleTrigger("trigger1", "group1", "job1", "group1", ts, null,
                                                    SimpleTrigger.RepeatIndefinitely, interval);//每若干小时运行一次，小时间隔由appsettings中的IndexIntervalHour参数指定

            //Timer：更加精准。
            sched.AddJob(job, true);
            sched.ScheduleJob(trigger);
            sched.Start();
            #endregion


            #region 定时发送给,注册用户：提示邮箱激活
            //ISchedulerFactory sf1 = new StdSchedulerFactory();  //创建一个计划工厂

            //IScheduler sched1 = sf1.GetScheduler();             //创建一个计划着

            //JobDetail job1 = new JobDetail("job2",              //任务名
            //                              "group2",             //任务组
            //                   typeof(SendNewRegUserEmailJob)   //执行哪个Job,、执行任务类,实现了IJob接口的类   
            //                              );

            //DateTime ts1 = TriggerUtils.get//.GetNextGivenSecondDate(null, 1);//1秒后开始
            //TimeSpan interval1 = TimeSpan.FromSeconds(2);//开始间隔10秒，依次发送   //创建定时

            ////每若干小时运行一次，小时间隔由appsettings中的IndexIntervalHour参数指定
            //Trigger trigger1 = new SimpleTrigger("trigger2",   //触发器名
            //                                        "group2",  //触发器组
            //                                        "job2",
            //                                        "group2",
            //                                         ts1,
            //                                         null,
            //                                        SimpleTrigger.RepeatIndefinitely,
            //                                        interval1);

            ////相比，Timer：，Quartz ,更加精准。
            //sched1.AddJob(job1, true);
            //sched1.ScheduleJob(trigger1);
            //sched1.Start();
            #endregion


            #region 定时发送给，注册用户，重置密码

            #endregion
        }
    }
}
