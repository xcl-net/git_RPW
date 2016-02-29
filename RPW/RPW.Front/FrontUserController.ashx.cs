
using RPW.Bll;
using RPW.Model;
using RPWCommonts;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
//using Maticsoft.Common;

namespace RPW.Front
{
    /// <summary>
    /// Login 的摘要说明
    /// </summary>
    public class FrontUserController : BaseHandler
    {
        public const string FRONTUSERACTIVECODE_PREFIX = "FrontUser.ActiveCode";//注册redis常量


        #region 进入注册界面
        public void register(HttpContext context)
        {
            context.Response.Redirect("~/Register.shtml");

        }
        #endregion

        #region 账号注册
        public void registerSubmit(HttpContext context)
        {
            string username = context.Request["username"];
            string password = context.Request["pwd"];
            string email = context.Request["email"];
            string validCode = context.Request["validCode"];
            string telephone = context.Request["telephone"];

            if (string.IsNullOrWhiteSpace(username))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "用户名不能为空！");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "密码不能为空！");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "邮箱地址不能为空！");
                return;
            }
            if (string.IsNullOrWhiteSpace(telephone))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "手机号不能为空！");
                return;
            }
            if (string.IsNullOrWhiteSpace(validCode))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "验证码不能为空！");
                return;
            }
            //检查邮箱地址

            if (!FrontHelper.IsEmail(email))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "邮箱地址格式不正确！");
                return;
            }
            //检查手机格式
            if (!FrontHelper.IsTelephone(telephone))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "手机号格式不正确！");
                return;
            }

            //检验用户名是否可用
            //通过js进行数据的合法性验证，只是为了用户用起来方便而已
            //在服务器端中的校验，才能保证数据的安全
            if (validCode != RPWCommonts.RPCommonHelper.GetValidCode(context))
            {

                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "验证码错误！");
                return;
            }

            //根据用户名验证
            FrontUsersBll userBll = new FrontUsersBll();
            if (!userBll.CheckUserNameOnReg(username))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "此用户名已经被使用！");
                return;
            }
            //验证，通过，注册！
            long userId = userBll.AddFrontUser(username, password, email, telephone);

            #region 使用关系数据库保存激活码（写入）
            /*
            //将激活码插入到，数据库激活码表中
            ActiveCodes actCode = new ActiveCodes();
            string activeCode = FrontHelper.CreateActveCode();
            actCode.Name = username;
            actCode.ActiveCode = activeCode;
            actCode.CreateTime = DateTime.Now;//方便激活时候查询时间差
            new ActiveCodesBll().Add(actCode);
            */

            #endregion


            #region  使用redis来改造注册码（写入）
            string activeCode = FrontHelper.CreateActveCode();
            using (IRedisClient client = RedisManager.ClientManager.GetClient())
            {
                //将注册码写入到redis中,键为了防止冲突。加上前缀，和当前用户名
                client.Set<string>(FRONTUSERACTIVECODE_PREFIX + username, activeCode, DateTime.Now.AddMinutes(30));//设定30分钟
            }
            #endregion

            //发送邮件（升级）
            //放到消息队列中，由定时服务器，去发送激活邮件操作，这里只是完成写入队列操作，大大节省了注册时间；
            //FrontHelper.SendEmaiSubjBody(username, activeCode, email);
            using (var client = RedisManager.ClientManager.GetClient())
            {
                string info = username + "," + activeCode + "," + email;
                client.EnqueueItemOnList("ActiveUser", info);
            }

            //存到消息队列中，通知管理员是谁注册了
            using (var client = RedisManager.ClientManager.GetClient())
            {
                string info = "姓名：" + username + "，邮箱：" + email + "手机号：" + telephone;
                client.EnqueueItemOnList("NewRegUser", info);
            }

            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "恭喜您，注册成功！请打开邮箱激活您的账户！");

        }
        #endregion

        #region 账号激活
        public void active(HttpContext context)
        {

            #region 使用关系数据保存激活码做法（读取）
            /*
            string activeCode = context.Request["activeCode"];
            string name = context.Request["username"];
            string activeTime = DateTime.Now.ToString();

            //根据用户名，查询激活码表，是否有这个用户的激活码信息
            ActiveCodesBll codeBll = new ActiveCodesBll();
            ActiveCodes codeInfo = codeBll.GetCodeByName(name);
            if (codeInfo == null)
            {
                FrontHelper.OutPutError(context, "用户名," + name + ",不存在！");//防止，在激活之前，数据库，已经删除了，注册的用户的情况
                return;
            }

            //到用户表中中，产看字段的IsActive查看是否是激活的
            FrontUsers userFront = new FrontUsersBll().GetModelByUserName(name);
            if (userFront.IsActive)
            {
                //提示用户，该用户已经激活，不需要再次激活了
                FrontHelper.OutPutMsg(context, "您的用户名，已经激活成功，不需要重复激活！");
                return;
            }

            //计算，激活时间，与注册时候发送“激活链接”的差值
            string zhuceTime = codeInfo.CreateTime.ToString();
            long timeValue = new ActiveCodesBll().TimeValue(zhuceTime, activeTime);
            long timeChaZhi = timeValue;
            if (timeChaZhi > 1)
            {
                FrontHelper.WhenActiveFile(name);// 激活失败，重新发送激活链接

                FrontHelper.OutPutMsg(context, "当前激活链接已经失效！新的激活链接已经发送到您的邮箱，请刷新邮箱获取新的激活链接，进行激活，30分钟内有效！");

                return;

            }

            //激活码，是数据库中的激活码，激活操作
            //更新FrontUser中的IsActive字段
            new FrontUsersBll().Active(name);

             */

            #endregion



            #region 使用redis方式读取注册激活码（读取）

            string activeCode = context.Request["activeCode"];
            string name = context.Request["username"];
            //对name解密；（实际上上是为了方便汉字传输的编码）
            name = RPWCommonts.RPCommonHelper.DesDecrypt(name);

            //到用户表中中，产看字段的IsActive查看是否是激活的
            FrontUsers userFront = new FrontUsersBll().GetModelByUserName(name);
            if (userFront.IsActive)
            {
                //提示用户，该用户已经激活，不需要再次激活了
                FrontHelper.OutPutMsg(context, "您的用户名，已经激活成功，不需要重复激活！");
                return;
            }

            using (IRedisClient client = RedisManager.ClientManager.GetClient())
            {
                //读取
                string activeCodeInRedis = client.Get<string>(FRONTUSERACTIVECODE_PREFIX + name);
                //判断
                if (activeCodeInRedis == null || activeCodeInRedis != activeCode)
                {
                    FrontHelper.WhenActiveFileByRedis(name);
                    FrontHelper.OutPutMsg(context, "当前激活链接已经失效或不正确！新的激活链接已经发送到您的邮箱，请刷新邮箱获取新的激活链接，进行激活，30分钟内有效！");
                    return;

                }
            }
            //激活码，是NOSql数据库中的激活码，激活操作
            //更新FrontUser中的IsActive字段
            new FrontUsersBll().Active(name);

            #endregion

            //激活成功
            //查询用户表中的激活状态
            FrontUsers userFrontFirst = new FrontUsersBll().GetModelByUserName(name);
            if (userFrontFirst.IsActive)
            {
                FrontHelper.OutPutMsg(context, "恭喜您，激活成功！");
            }
            else//激活失败，要再次发送激活链接
            {
                FrontHelper.WhenActiveFile(name);// 激活失败，重新发送激活链接

                FrontHelper.OutPutMsg(context, "激活失败！新的激活链接已经发送到您的邮箱，请刷新邮箱获取新的激活链接，进行激活，30分钟内有效！");

            }
        }
        #endregion

        #region 账号登陆,从哪里请求的，就返回到那个url地址的页面去

        public void index(HttpContext context)
        {
            //检查Cookie是否有记住的用户名和密码,尝试自动登录
            if (FrontHelper.TryAutoLogin(context))
            {
                //从哪里请求的，就返回到那个url地址的页面去
                string navUrl = (string)context.Session[FrontHelper.LOGINBEFOREURL];
                //如果登陆前的地址有，就重定向到登录前的页面
                if (navUrl != null)
                {
                    context.Response.Redirect(navUrl);
                }
                context.Response.Redirect("~/index.shtml");//进入系统后台
                return;
            }
            RPWRazor.RPRazorHelper.OutputRazor(context, "/AdminUser/login.shtml");
            //加载完，返回
            return;
        }
        #endregion

        #region 账号登录

        public void login(HttpContext context)
        {
            #region 登录请求获取的参数
            context.Response.ContentType = "application/json";  //登陆的校验时用的ajax进行
            string username = context.Request["username"];
            string password = context.Request["password"];
            string autologin = context.Request["autologin"];
            string verificationCode = context.Request["validCode"];

            if (string.IsNullOrWhiteSpace(verificationCode))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "验证码不能为空！");
                return;
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "用户名不能为空！");
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "密码不能为空！");
                return;
            }


            //从Session中取到yzm
            string sessionVerificationCode = RPCommonHelper.GetValidCode(context);//获取服务器端生成的验证码

            if (verificationCode != sessionVerificationCode)//(用户输入的验证码==服务器端的验证码)吗？
            {
                RPCommonHelper.ResetYZM(context);//一旦不一致，就将验证码，进行在服务器端的重置。以防止客户端通过修改js，html等阻止验证码的刷新,然后进行暴力破解

                //本来客户端已经做了这个设置，但是用户不阻止了js的使用，那么这里生成的验证码，根本不会使用，这里就是对黑客专杀的。。。

                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "验证码错误！");
                return;
            }
            #endregion

            #region 前台用户登录结果校验，只是将前台的登录成功，那种情况的用户名，放到session中
            //为了明显的三层，，业务逻辑代码都放在Bll层中 --杨sir说。
            //转到Bll。ext。cs中去写代码。
            //调用Bll层中自己封装的方法
            FrontLoginResult result = new FrontUsersBll().Login(username, password);

            if (result == FrontLoginResult.UserNameNotFound)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "用户名不存在！");
                return;
            }
            else if (result == FrontLoginResult.PasswordError)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "密码错误！");
                return;
            }
            else if (result == FrontLoginResult.UserNameDisActive)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "该用户名未激活！");
                return;
            }
            else if (result == FrontLoginResult.OK)
            {

                //如果是登录成功的话，就将用户名和加密的密码放到 Cookie中，加密算法使用Des加密算法；
                //密码不能是明文保存，否则危险；
                //如果自动登录是“on”；
                if (autologin == "on")
                {
                    //如果是自动登录勾选的话
                    //保存，用户名，密码 到 Cookie 中
                    //
                    FrontHelper.Remember(context, username, password);

                }

                //取得用户的id
                FrontUsers user = new FrontUsersBll().GetModelByUserName(username);
                //记到Session中，同时，用户名字也存到session中,方便其他用户的调用
                FrontHelper.StoreInSession(context, user.Id, user.Name);

                //登录成功，保存到redis数据库中，，同样，自动登录也需要相同的代码
                using (IRedisClient client = RedisManager.ClientManager.GetClient())
                {
                    client.Set<string>(FrontHelper.FRONTSESSIONID_PREFIX + user.Name, context.Session.SessionID);
                }
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "登录成功！");

            }
            else
            {
                throw new Exception("登陆结果未知！" + result);
            }
            #endregion
        }
        #endregion

        #region 账号退出
        public void loginOut(HttpContext context)
        {
            //退出登录，逻辑代码还是写在FrontHelper中
            context.Session.Abandon();//销毁Session
            FrontHelper.Forget(context);//销毁Cookie
            context.Response.Redirect("login.shtml");//返回到登录界面
        }
        #endregion

        #region 检查用户Cookie信息，是不是存在：实现自动登录用户名的功能；检查Sessino中是否存在，用户的登录信息，有，则是登录的状态
        public void IsLogin(HttpContext context)
        {
            //这是第一次，从session中，获取用户名
            string username = FrontHelper.GetUserNameInSession(context);

            //根据，session是否有用户名，来判断用户是不是，登录的状态
            if (string.IsNullOrWhiteSpace(username))//这里是用户名是null
            {
                //检查，用户名，不存在，尝试自动登录（用户是不是有Cookie信息，存在，有的话，就自动登录）
                bool loginResult = FrontHelper.TryAutoLogin(context);
                if (loginResult)
                {
                    //自动登录会重新存用户名到session中
                    //再次获取用户的名字，从Session中
                    string usernameInSession = FrontHelper.GetUserNameInSession(context);
                    //尝试自动登录成功，将用户名传递到前台页面
                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "yes", usernameInSession);
                }
                else
                {
                    //尝试自动登录失败，ajax返回结果
                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "no", "");
                }

            }
            else//这里是用户名不为null
            {
                //用户登录了，将用户名传递到前台页面
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "yes", username);
            }

        }
        #endregion

        #region 注册时候，用户输入新的用户名，检查用户是否可以使用功能

        public void checkUserName(HttpContext context)
        {
            string username = context.Request["username"];

            FrontUsersBll userBll = new FrontUsersBll();
            if (!userBll.CheckUserNameOnReg(username))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "此用户名已经被使用！");
                return;
            }
            else
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "此用户名可用！");
            }

        }
        #endregion

        #region 忘记密码重置功能
        //找回密码第一步
        public void confirmEmail(HttpContext context)
        {
            string email = context.Request["email"];
            string validCode = context.Request["validCode"];

            if (string.IsNullOrWhiteSpace(email))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "邮箱地址不能为空！");
                return;
            }

            if (string.IsNullOrWhiteSpace(validCode))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "验证码不能为空！");
                return;
            }
            //检查邮箱地址

            if (!FrontHelper.IsEmail(email))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "邮箱地址格式不正确！");
                return;
            }

            if (validCode != RPWCommonts.RPCommonHelper.GetValidCode(context))
            {

                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "验证码错误！");
                return;
            }
            //根据邮箱地址，查找数据库中是否，有这个用户的存在
            FrontUsers user = new FrontUsersBll().GetModelByEmail(email);
            if (user == null)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "不存在该用户");
                return;
            }
            //加密显示
            //string newEmail = email.Substring(0, 3) + "*****" + email.Substring(email.Length - 4, 4);
            //都通过
            //加载下一步
            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", email);
        }

        //找回密码第二步
        public void serNext(HttpContext context)
        {
            string email = context.Request["email"];
            RPWRazor.RPRazorHelper.OutputRazor(context, "~/forgetyz.cshtml", email);
        }

        public void sentValidCode(HttpContext context)
        {
            //发给谁邮件
            string email = context.Request["email"];
            FrontUsers user = new FrontUsersBll().GetModelByEmail(email);
            string name = user.Name;
            //邮件内容
            FrontHelper.SendEmaiOnBackPwd(name, email);
            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "验证码已发送");
        }
        //找回密码第三步
        public void confirmValidCode(HttpContext context)
        {

            string email = context.Request["email"];
            string numValidCode = context.Request["numValidCode"];

            //验证
            FrontUsers user = new FrontUsersBll().GetModelByEmail(email);
            string name = user.Name;
            string CacheKey = "ActiveCode" + name;//注册时候设置的缓存id
            string ActiveCodeInCache = (string)Maticsoft.Common.DataCache.GetCache(CacheKey);
            if (numValidCode != ActiveCodeInCache)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "数字验证码错误，请点击重新获取验证码！");
                return;
            }
            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", email);//ok把email地址传递到第三步中
        }

        //加载第三步页面
        public void loadSetPwd(HttpContext context)
        {
            string email = context.Request["email"];
            FrontUsers user = new FrontUsersBll().GetModelByEmail(email);
            RPWRazor.RPRazorHelper.OutputRazor(context, "~/fogetNewPassword.cshtml", new { email = email, name = user.Name });

        }
        //第三步页面ajax请求
        public void setPwd(HttpContext context)
        {

            string email = context.Request["email"];
            string pwd = context.Request["pwd"];
            FrontUsers user = new FrontUsersBll().GetModelByEmail(email);
            user.Pwd = RPCommonHelper.CalcMD5(pwd);
            new FrontUsersBll().Update(user);
            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "密码重置成功！");
        }
        #endregion

        #region  加载报名界面
        public void loadApplication(HttpContext context)
        {

            //加载课程的下拉列表
            CoursesBll courseBll = new CoursesBll();
            List<Courses> courses = courseBll.GetModelList("");
            long[] selecetValue = new long[] { 6 };


            RPWRazor.RPRazorHelper.OutputRazor(context, "~/Application.cshtml",
                new { courses = courses, selecetValue = selecetValue });
        }
        #endregion

        #region 报名功能
        public void application(HttpContext context)
        {
            string name = context.Request["name"];
            string telephone = context.Request["telephone"];
            string qqNum = context.Request["qqNum"];
            string school = context.Request["school"];
            string course = context.Request["course"];

            if (string.IsNullOrWhiteSpace(name))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "请填写您的姓名");
                return;
            }
            if (string.IsNullOrWhiteSpace(telephone) && string.IsNullOrWhiteSpace(qqNum))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "请填写您的手机号或者QQ号，以便我们能及时的联系到您");
                return;
            }

            if (string.IsNullOrWhiteSpace(school))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "请填写您的学校");
                return;
            }
            if (string.IsNullOrWhiteSpace(course))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "请填写您的想学习的课程");
                return;
            }
            //执行 增
            ApplicationsBll applyBll = new ApplicationsBll();
            Applications applys = new Applications();
            applys.Name = name;
            applys.Telephone = telephone;
            applys.QQ = qqNum;
            applys.School = school;
            applys.Course = course;
            applyBll.Add(applys);
            //邮件通知给谁
            string who = ConfigurationManager.AppSettings["WhoReceive"];
            string subject = "如鹏消息：新用户:" + name + "报名成功！";
            string mailcontent = "新用户：[" + name + "]报名成功，电话号码是：" + telephone + ",QQ号是：" + qqNum + ",学校是：" + school + ",所报的课程是：" + course;

            //发送邮件
            FrontHelper.SendEmail(who, subject, mailcontent);
            RPAjaxhelperCommons.WriteJson(context.Response, "ok", "报名成功！我们将以最快的时间联系您！");
        }
        #endregion

        #region 加载页面的时候，加载用户完善的信息
        public void ShowUserInfo(HttpContext context)
        {

            string username = FrontHelper.GetUserNameInSession(context);
            //判断用户是否是登录状态，否则，以下都不能执行
            if (username == null)
            {
                context.Response.Redirect("/login.shtml");
                return;
            }
            FrontUsers frontUser = new FrontUsersBll().GetModelByUserName(username);
            //学历表
            List<Graduates> graduates = new GraduatesBll().GetModelList("");
            //加载用户表的的学历
            int graduate = Convert.ToInt32(frontUser.Graduate);

            int idGraduate = graduate;

            //入学年份表
            List<EnrolYears> years = new EnrolYearsBll().GetModelList("");
            //查出当前用户的入学年份，在表T_FrontUsers中
            //如果有这个记录(用户输入过信息了)，就加载这个用户完善过的信息
            //否则，加载默认的年份

            int? enroalYear = frontUser.EnroalYear;//Model中年份存的是，年份表的id值

            if (enroalYear == 0 || enroalYear == null)//根据这个值，判断用户是第一次进入的这个页面：为什么会是0，应为用户在注册的时候，这个字段一定是空的，故0；
            {

                //加载信息
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/ShowUserInfo.cshtml",
               new
               {
                   realName = "",
                   schoolName = "",
                   specialty = "",
                   qq = "",
                   teleNum = frontUser.Telephone,
                   email = frontUser.Email,
                   enroalYear = years,
                   graduate = graduates,
                   defaultYear = new long[] { 6 },//加载默认的年份
                   defaultGraduate = new int[] { idGraduate }//加载用户表中的学历字段，这个是必填的因为
               });
            }
            else
            {
                //用户完善过的，真实姓名
                string realName = frontUser.RealName;
                //用户写过的，学校名称
                string schoolName = frontUser.SchoolName;
                //用户写过的，QQ号
                string qq = frontUser.QQ;
                //用户写过的，专业
                string special = frontUser.Specialty;
                int id = (int)enroalYear;
                //加载信息
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/ShowUserInfo.cshtml",
               new
               {
                   realName = realName,
                   schoolName = schoolName,
                   specialty = special,
                   qq = qq,
                   teleNum = frontUser.Telephone,
                   email = frontUser.Email,
                   enroalYear = years,
                   graduate = graduates,
                   defaultYear = new int[] { id },
                   defaultGraduate = new int[] { idGraduate }//加载用户表中的学历字段，这个是必填的因为
               });

            }
        }
        #endregion

        #region 完善信息
        public void CompleteUserInfo(HttpContext context)
        {
            string realname = context.Request["realname"];
            string school = context.Request["school"];
            string enrolYear = context.Request["enrolYear"];
            string specialty = context.Request["specialty"];
            string graduate = context.Request["graduate"];
            string qq = context.Request["qq"];

            if (string.IsNullOrWhiteSpace(realname))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "真实姓名必须填写");
                return;
            }
            if (string.IsNullOrWhiteSpace(school))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "所在学校必须填写");
                return;
            }
            if (string.IsNullOrWhiteSpace(enrolYear))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "入学年份必须填写");//不要也行，默认选项
                return;
            }
            if (string.IsNullOrWhiteSpace(specialty))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "所学专业必须填写");
                return;
            }
            if (string.IsNullOrWhiteSpace(graduate))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "学历必须填写");//不要也行，默认选项
                return;
            }



            string name = FrontHelper.GetUserNameInSession(context);
            FrontUsers frontUser = new FrontUsersBll().GetModelByUserName(name);
            frontUser.RealName = realname;
            frontUser.SchoolName = school;
            frontUser.EnroalYear = Convert.ToInt32(enrolYear);
            frontUser.Specialty = specialty;
            frontUser.Graduate = Convert.ToInt32(graduate);
            frontUser.QQ = qq;

            //更新信息
            new FrontUsersBll().Update(frontUser);
            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "修改成功");
        }
        #endregion

        #region 查看谁在学习
        public void WhoLearning(HttpContext context)
        {
            //查出T_WhoLearning的前 100 条记录
            //需要自己写一个方法
            List<WhoLernings> whoModels = new WhoLerningsBll().GetListShowTopByCache(100, "1=1", "Id");
            if (whoModels.Count == 0)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "没有记录", null);
                return;
            }
            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "", whoModels);

        }
        #endregion

    }
}