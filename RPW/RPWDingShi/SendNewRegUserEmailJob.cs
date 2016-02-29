using Quartz;
using RPWCommonts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RPWNewRegDingShi
{
    public class SendNewRegUserEmailJob : IJob
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">要发送的邮箱</param>
        /// <param name="mailSubject">邮箱主题</param>
        /// <param name="mailContent">邮箱内容</param>
        /// <returns>返回发送邮箱的结果</returns>
        public static void SendEmail(string mailTo, string mailSubject, string mailContent)
        {
            // 设置发送方的邮件信息,例如使用网易的smtp
            string smtpServer = ConfigurationManager.AppSettings["SmtpServer"]; //SMTP服务器
            string mailFrom = ConfigurationManager.AppSettings["MailFrom"]; //登陆用户名  
            string userPassword = ConfigurationManager.AppSettings["UserPassword"];//登陆密码    

            // 邮件服务设置
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = smtpServer; //指定SMTP服务器
            smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, userPassword);//用户名(可能不用带@163.com)和密码

            // 发送邮件设置        
            MailMessage mailMessage = new MailMessage(mailFrom, mailTo); // 发送人和收件人
            mailMessage.Subject = mailSubject;//主题
            mailMessage.Body = mailContent;//内容
            mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
            mailMessage.IsBodyHtml = true;//设置为HTML格式
            mailMessage.Priority = MailPriority.Normal;//优先级

            try
            {
                smtpClient.Send(mailMessage); // 发送邮件

            }
            catch (SmtpException ex)
            {
                throw new Exception("邮件发送失败", ex);

            }
        }

        public void Execute(JobExecutionContext context)
        {
            string whoReceivedEmai = ConfigurationManager.AppSettings["AdminReceived"];
            //执行工作；
            using (var client = RedisManager.ClientManager.GetClient())
            {
                //声明一个list集合
                List<string> infos = new List<string>();
                //每次向用户发送数据的时候，将存入list集合中的所有的用户信息发送给管理员；
                //发送完毕，结束Job

                //使用一个while循环完成，不断的从队列中取消息
                while (true)
                {
                    //取出来，一条用户的消息；
                    string info = client.DequeueItemFromList("NewRegUser");


                    if (info == null)//从消息队列中取出为空，就开始发送
                    {
                        //如果，从队列中，不能取数据了；
                        //则开始发送邮件；

                        //发送邮件，从list集合中，取出全部注册的用户数据，拼接成一个字符串；
                        if (infos.Count != 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("<ul>");
                            foreach (var inf in infos)
                            {
                                sb.Append("<li>" + inf + "</li>");
                            }
                            sb.Append("</ul>");
                            infos.Clear(); //取出完毕，清除；
                            SendEmail(whoReceivedEmai, "新注册用户信息", sb.ToString());

                        }
                        else
                        {
                            return;  //如果从list中发现也是为空的就直接返回方法到Execute（），结束Job；       
                        }


                    }
                    else//队列不为空，就存到list中，直到队列为空，就停止存入list；
                    {
                        infos.Add(info);
                    }
                }
            }
        }
    }
}
