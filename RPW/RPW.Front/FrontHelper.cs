using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RPWRazor;
using RPW.Model;
using RPW.Bll;
using RPWCommonts;
using System.Net.Mail;
using System.Text;
using System.Configuration;
using ServiceStack.Redis;
namespace RPW.Front
{
    public class FrontHelper
    {
        public const string FRONTSESSIONID_PREFIX = "FrontUserSessionId.";

        #region 前台页面出错了，统一调用这个方法
        public static void OutPutError(HttpContext context, string msg)
        {

            RPRazorHelper.OutputRazor(context, "~/Error.cshtml", msg);
        }
        #endregion


        #region 前台页面返回消息，统一调用这个方法
        public static void OutPutMsg(HttpContext context, string msg)
        {

            RPRazorHelper.OutputRazor(context, "~/Msg.cshtml", msg);
        }
        #endregion


        #region 由段落表 根据给定的章节号，查询出，对应的段落名称列表
        public static List<Segments> GetSegments(long chapterId)
        {
            SegmentsBll bll = new SegmentsBll();
            return bll.GetModelListByCache("ChapterId=" + chapterId);

        }
        #endregion


        #region 建立字段防止名字写错，方便读取
        private const string USERNAME = "FrontUserName";
        private const string PASSWORD = "FrontUserPassWord";
        private const string USERID = "UserId";
        public const string LOGINBEFOREURL = "loginTryUrl";//尝试登陆时候的页面地址 
        #endregion


        #region 记住用户名和密码到Cookie 中
        public static void Remember(HttpContext context, string username, string password)
        {
            context.Response.SetCookie(new HttpCookie(USERNAME, context.Server.UrlEncode(username)) { Expires = DateTime.Now.AddDays(30) });
            context.Response.SetCookie(new HttpCookie(PASSWORD, RPCommonHelper.DesEncypt(password)) { Expires = DateTime.Now.AddDays(30) });//密码加密存入cookie

        }
        #endregion


        #region 设置当前登录的，用户Id，用户名存到Session中
        public static void StoreInSession(HttpContext context, long userId, string username)
        {
            context.Session[USERID] = userId;
            context.Session[USERNAME] = username;
        }
        #endregion


        #region 取出当前登录的，用户Id，从Session中
        public static long? GetUserIdInSession(HttpContext context)//从session中可能取到的是空，所以为int？可空类型
        {
            return (long?)context.Session[USERID];
        }
        #endregion


        #region 取出当前登录的，用户名，从session中
        public static string GetUserNameInSession(HttpContext context)
        {
            string userName = (string)context.Session[USERNAME];
            return userName;
        }
        #endregion


        #region 忘记Cookie中存储的用户名。密码
        public static void Forget(HttpContext context)
        {
            //不是删除的同一个，cookie信息，报异常
            HttpCookie cookieUserName = context.Request.Cookies[USERNAME];

            if (cookieUserName != null)
            {
                string usernameOfKey = cookieUserName.Name;
                if (USERNAME != usernameOfKey)
                {
                    throw new Exception("不能删除cookie信息,查看是否是同一个cookie键");
                }
            }
            //过期时间设置为已经过去的时间，就能起到删除cookie的作用
            context.Response.SetCookie(new HttpCookie(USERNAME, "") { Expires = DateTime.Now.AddDays(-1) });
            context.Response.SetCookie(new HttpCookie(PASSWORD, "") { Expires = DateTime.Now.AddDays(-1) });
        }
        #endregion


        #region 如果用户点击了，记住用户名的功能，就尝试自动登录
        public static bool TryAutoLogin(HttpContext context)
        {
            //读取用户名，密码
            HttpCookie cookieUserName = context.Request.Cookies[USERNAME];
            HttpCookie cookiePassWord = context.Request.Cookies[PASSWORD];
            //尝试登陆
            if (cookieUserName != null && cookiePassWord != null)//用户名和密码都不能为空
            {
                string username = context.Server.UrlDecode(cookieUserName.Value);
                string passwordDes = cookiePassWord.Value;
                //对密码进行解密
                string password = RPCommonHelper.DesDecrypt(passwordDes);
                //密码解密的过程，可能会，抛异常：因为cookie 信息被软件改掉，用户修改，对解密失败，进行捕获异常！

                //用户名，密码解密正确，开始登陆
                FrontUsersBll bll = new FrontUsersBll();
                FrontLoginResult loginResult = bll.Login(username, password);
                if (loginResult == FrontLoginResult.OK)
                {
                    //根据用户名，获得当前用户的model
                    FrontUsers user = bll.GetModelByUserName(username);
                    //将用户的id和用户的名字存到Session中
                    FrontHelper.StoreInSession(context, user.Id, user.Name);
                    //将登录的sessionId信息存到redis数据库中
                    using (IRedisClient client = RedisManager.ClientManager.GetClient())
                    {
                        client.Set<string>(FrontHelper.FRONTSESSIONID_PREFIX + user.Name, context.Session.SessionID);
                    }
                    return true;
                }
                else
                {
                    return false;
                }


            }
            else//用户名，密码是空的有
            {
                return false;
            }

        }
        #endregion



        #region 邮箱正则
        public static bool IsEmail(string email)
        {

            return System.Text.RegularExpressions.
                //使用正则表达式判断是否匹配  

               Regex.IsMatch(email, @"^(([\w\.]+)@(([[0-9]{1,3}\.[0-9]{1,3}\. [0-9]{1,3}))|((\w+\.?)+)@([a-zA-Z]{2,4}| [0-9]{1,3})(\.[a-zA-Z]{2,4}))$");

        }
        #endregion


        #region 手机号正则
        public static bool IsTelephone(string str_telephone)
        {

            return System.Text.RegularExpressions.Regex.
                //使用正则表达式判断是否匹配  

                IsMatch(str_telephone, @"^[1][3-5]\d{9}$");
        }
        #endregion


        #region 生成随机数，做为激活码
        public static string CreateActveCode()
        {
            Random rand = new Random();
            return rand.Next(10000, 99999).ToString();
        }
        #endregion


        #region  激活失败，重新发送激活链接
        public static void WhenActiveFile(string name)
        {
            //根据用户名，查询激活码表，是否有这个用户的激活码信息
            ActiveCodesBll codeBll = new ActiveCodesBll();
            ActiveCodes codeInfo = codeBll.GetCodeByName(name);
            //更新该用户的激活码，并将激活码，发送到用户的邮箱，提示重新激活
            string newActiveCode = FrontHelper.CreateActveCode();
            codeInfo.CreateTime = DateTime.Now;
            codeInfo.ActiveCode = newActiveCode;
            codeInfo.Name = name;
            //更新
            new ActiveCodesBll().Update(codeInfo);
            //发送新的激活连接
            //获取，用户邮件地址
            FrontUsers userf = new FrontUsersBll().GetModelByUserName(name);
            string email = userf.Email;
            //再次，发送用户的激活码，并且，提示用户等待激活！
            //----------------------------------------- FrontHelper.SendEmaiSubjBody(name, newActiveCode, email);
        }
        #endregion


        #region 使用redis，激活失败，重新发送激活链接
        public static void WhenActiveFileByRedis(string name)
        {
            string newActiveCode = FrontHelper.CreateActveCode();
            //发送新的激活连接
            //获取，用户邮件地址
            FrontUsers userf = new FrontUsersBll().GetModelByUserName(name);
            string email = userf.Email;
            //再次，发送用户的激活码，并且，提示用户等待激活！
            //----------------------------------- FrontHelper.SendEmaiSubjBody(name, newActiveCode, email);
        }
        #endregion


        #region 找回密码
        public static void SendEmaiOnBackPwd(string username, string email)
        {

            string CacheKey = "ActiveCode" + username; //缓存的id(根据传入的用户名，设置缓存id，防止：同时注册了多个用户，激活的时候，不知道读取那个缓存了的情况)
            object objCode = Maticsoft.Common.DataCache.GetCache(CacheKey);  //设置一个缓存

            if (objCode == null)  //如果缓存为空（超过30）
            {
                string numactiveCode = FrontHelper.CreateActveCode();
                //存到缓存中
                objCode = numactiveCode;

                if (objCode != null)  //缓存不为空
                {
                    int numActiveCodeCache = Maticsoft.Common.ConfigHelper.GetConfigInt("numActiveCodeCache"); //设置缓存时间
                    Maticsoft.Common.DataCache.SetCache(CacheKey, objCode, DateTime.Now.AddMinutes(numActiveCodeCache), TimeSpan.Zero);
                    //                                   缓存id     缓存实体对象    过期时间（从当前时间加分钟）  时间跨度
                }


                //将激活码发送到用户的邮箱，发送的内容是
                string emailBody = "尊敬的" + username + ",您好,验证码是：" + numactiveCode + ",请及时验证，30分钟内有效！";
                string mailTo = email;
                string subject = "重置密码验证码";
                SendEmail(mailTo, subject, emailBody);
            }
        }
        #endregion

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
    }
}