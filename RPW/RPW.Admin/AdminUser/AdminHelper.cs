using RazorEngine.Text;
using RPW.Bll;
using RPW.Model;
using RPWCommonts;
using RPWRazor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace RPW.Admin.AdminUser
{
    public class AdminHelper
    {
        //建立字段防止名字写错，方便读取
        private const string USERNAME = "UserName";
        private const string PASSWORD = "PassWord";
        private const string USERID = "UserId";
        public const string LOGINBEFOREURL = "loginTryUrl";//尝试登陆时候的页面地址

        /// <summary>
        /// 记住用户名和密码到Session中
        /// </summary>
        /// <param name="context"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void Remember(HttpContext context, string username, string password)
        {
            context.Response.SetCookie(new HttpCookie(USERNAME, username) { Expires = DateTime.Now.AddDays(7) });
            context.Response.SetCookie(new HttpCookie(PASSWORD, RPCommonHelper.DesEncypt(password)) { Expires = DateTime.Now.AddDays(7) });

        }



        /// <summary>
        /// 用户Id，用户名存到Session中
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        /// <param name="username"></param>
        public static void StoreInSession(HttpContext context, long userId, string username)
        {
            context.Session[USERID] = userId;
            context.Session[USERNAME] = username;
        }
        /// <summary>
        /// 取出用户Id，从Session中
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static long? GetUserIdInSession(HttpContext context)//从session中可能取到的是空，所以为int？可空类型
        {
            return (long?)context.Session[USERID];
        }
        /// <summary>
        /// 取出用户名，从Session中
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserNameInSession(HttpContext context)
        {
            return (string)context.Session[USERNAME];
        }

        /// <summary>
        /// 尝试自动登录
        /// </summary>
        /// <param name="context"></param>
        /// <returns>是否登录成功</returns>
        public static bool TryAutoLogin(HttpContext context)
        {
            //读取用户名，密码
            HttpCookie cookieUserName = context.Request.Cookies[USERNAME];
            HttpCookie cookiePassWord = context.Request.Cookies[PASSWORD];
            //尝试登陆
            if (cookieUserName != null && cookiePassWord != null)//用户名和密码都不能为空
            {
                string username = cookieUserName.Value;
                string passwordDes = cookiePassWord.Value;
                //对密码进行解密
                string password = RPCommonHelper.DesDecrypt(passwordDes);
                //密码解密的过程，可能会，抛异常：因为cookie 信息被软件改掉，用户修改，对解密失败，进行捕获异常！

                //用户名，密码解密正确，开始登陆
                AdminUsersBll bll = new AdminUsersBll();
                LoginResult loginResult = bll.Login(username, password);
                if (loginResult == LoginResult.OK)
                {
                    //尝试登陆成功，将用户id和用户名放到session中
                    AdminUsers user = bll.GetByUserName(username);
                    AdminHelper.StoreInSession(context, user.Id, user.UserName);
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
        /// <summary>
        /// 忘记Cookie中存储的用户名。密码
        /// </summary>
        /// <param name="context"></param>
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



            /* 做测试的代码，结果这里还是可以检测到，存入的用户名和密码，就应该想到，删除cookie没有成功，删除没有成功，
             * 就应该想到，删除的不是同一个名字的cookie信息，所以应该看报文，存入的cookie键是不是一致的。
             * 结果，发现，cookie键不一样，所以我想，在消除cookie信息的时候，来一个，不是同一个cookie键的让他报异常！！


            HttpCookie cookieUserName = context.Request.Cookies[USERNAME];
            string username = cookieUserName.Value;
            
            HttpCookie cookiePassWord = context.Request.Cookies[PASSWORD];
            string passwordDes = cookiePassWord.Value;
             */
        }
        /// <summary>
        /// 检查是否登录,是否有用户名
        /// </summary>
        /// <param name="context"></param>
        public static void CheckAccess(HttpContext context)
        {
            if (AdminHelper.GetUserIdInSession(context) == null)
            {
                context.Response.Redirect("/AdminUser/Login.ashx?action=index", true);//地址加上 斜杠， 表示是从根目录中调用的
                //重载 true 表示终止请求
                //endResponse:
                //     指示当前页的执行是否应终止。
                return;
                //多次调用，放到AdminHelper中
            }
        }
        /// <summary>
        /// 记录当前用户的操作记录到表T_AdministorOperationLogs中
        /// </summary>
        /// <param name="description">添加用户操作的描述</param>
        public static void RecordOperationLog(string description)
        {
            //获得当前登录用户的Id（可能拿不到当前的用户id）
            //从存入的session中获取
            long? userId = GetUserIdInSession(HttpContext.Current);
            if (userId == null)
            {
                //可能在登录的期间，session刚好过期了？？
                throw new Exception("当前用户没有登录！");
            }

            //获得当前登录的账户
            string userName = GetUserNameInSession(HttpContext.Current);
            AdministorOperationLogs logModel = new AdministorOperationLogs();

            logModel.UserId = (long)userId;
            logModel.OperateDate = DateTime.Now;
            logModel.Description = "“" + userName + "”，" + description;
            //日志表的添加方法
            new AdministorOperationLogsBll().Add(logModel);


        }
        /// <summary>
        /// 根据选中的用户选中的角色id的集合，返回选中各个角色的名字
        /// </summary>
        /// <param name="selectedRoles"></param>
        /// <returns></returns>
        public string UserHavedRolesName(string selectedRoles)
        {
            string strWhere = "Id in (" + selectedRoles + ")";
            List<Roles> roles = new RolesBll().GetModelList(strWhere);
            //遍历出来角色名字，拼接成一个字符串，返回到UI层
            StringBuilder rolesName = new StringBuilder();
            foreach (var role in roles)
            {
                rolesName.Append(" ").Append(role.Name).Append(" ");
            }
            return rolesName.ToString();
        }


        /// <summary>
        /// 根据选中的用户的id的集合，返回选中的用户的名字集合
        /// </summary>
        /// <param name="selectedUserIdList"></param>
        /// <returns></returns>
        public static string UserNames(string selectedUserIdList)
        {
            string strWhere = " Id in (" + selectedUserIdList + ")";
            List<AdminUsers> users = new AdminUsersBll().GetModelList(strWhere);
            //遍历出来用户名字，拼接成一个字符串，返回到UI层
            StringBuilder userNames = new StringBuilder();
            foreach (var user in users)
            {
                userNames.Append(" ").Append(user.UserName).Append(" ");
            }
            return userNames.ToString();
        }

        /// <summary>
        /// 权限id得到权限名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string PoweridToName(long id)
        {
            Powers power = new PowersBll().GetModel(id);
            return power.Name;
        }

        /// <summary>
        /// 根据选中的权限的id的集合，返回选中的权限的名字
        /// </summary>
        /// <param name="selectedPowerIdList"></param>
        /// <returns></returns>
        public static string powerNames(string selectedPowerIdList)
        {
            string strWhere = " Id in (" + selectedPowerIdList + ")";
            List<Powers> users = new PowersBll().GetModelList(strWhere);
            //遍历出来权限名字，拼接成一个字符串，返回到UI层
            StringBuilder powerName = new StringBuilder();
            foreach (var user in users)
            {
                powerName.Append(" ").Append(user.Name).Append(" ");
            }
            return powerName.ToString();
        }

        /// <summary>
        /// 角色id得到角色名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string RoleIdToName(long id)
        {
            Roles role = new RolesBll().GetModel(id);
            return role.Name;
        }

        /// <summary>
        /// 根据选中的角色的id的集合，返回选中的角色的名字集合
        /// </summary>
        /// <param name="roleIdList"></param>
        /// <returns></returns>
        public static string RoleIdsToNames(string roleIdList)
        {
            List<Roles> roles = new RolesBll().GetModelList(roleIdList);
            //遍历出来角色名字，拼接成一个字符串，返回到UI层
            StringBuilder rolesName = new StringBuilder();
            foreach (var role in roles)
            {
                rolesName.Append(" ").Append(role.Name).Append(" ");
            }
            return rolesName.ToString();
        }

        /// <summary>
        /// 判断当期用户,是否有该权限
        /// </summary>
        /// <param name="powerName"></param>
        /// <returns></returns>
        public static bool HasPower(string powerName)
        {
            //先获得当前登录用户的id
            //获得权限表中，权限id，
            //到角色--权限表中查询，哪些角色有这个权限
            //然后到 用户--角色 中判断，当前用户的是否拥有这些角色
            long? userId = GetUserIdInSession(HttpContext.Current);
            if (userId == null)
            {
                HttpContext.Current.Response.Redirect("/AdminUser/Login.ashx");
            }
            return new AdminUsersBll().HasPower(userId.Value, powerName);
        }
        /// <summary>
        /// 判断当期用户,是否有该权限
        /// </summary>
        /// <param name="powerName"></param>
        public static void CheckPower(string powerName)
        {
            if (!AdminHelper.HasPower(powerName))
            {
                HttpContext.Current.Response.Write("当前用户没有【" + powerName + "】的权限！");
                //没有权限，结束请求
                HttpContext.Current.Response.End();
            }
        }
        /// <summary>
        /// 当为批量按钮的权限，使用的是，json返回字符串
        /// </summary>
        /// <param name="context"></param>
        /// <param name="powerName"></param>
        public static void CheckPowerOnBatchButton(HttpContext context, string powerName)
        {
            if (!AdminHelper.HasPower(powerName))
            {
                //返回Json通知
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "当前用户没有【" + powerName + "】的权限！");
                //没有权限，结束请求
                HttpContext.Current.Response.End();

            }

        }
        /// <summary>
        /// 当加载选择条件页面的时候，同一调用这个方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="alertTxt"></param>
        /// <param name="keyList"></param>
        /// <param name="logs"></param>
        public static void LoadCshtml(HttpContext context, RawString alertTxt, List<KeyWords> keyList, List<AdministorOperationLogs> logs)
        {
            RPRazorHelper.OutputRazor(context, "~/AdminUser/LookUpLogSystem.cshtml",
                       new
                       {
                           //用户输入的用户名字
                           Uname = "",
                           KeyOfSerchs = keyList,
                           KeyCharcterId = new long[] { 0 },
                           Records = logs,
                           Alert = alertTxt
                       });

        }


    }
}