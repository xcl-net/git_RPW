using RPW.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using RPWCommonts;
using RPW.Model;

namespace RPW.Admin.AdminUser
{
    /// <summary>
    /// Login 的摘要说明
    /// </summary>
    public class Login : IHttpHandler, IRequiresSessionState //Session
    {


        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";//1.1
            //1.2 根据action来判断使用那个模板
            string action = context.Request["action"];
            if ("index" == action)
            {
                //检查Cookie是否有记住的用户名和密码,尝试自动登录
                if (AdminHelper.TryAutoLogin(context))
                {
                    //从哪里请求的，就返回到那个url地址的页面去
                    string navUrl = (string)context.Session[AdminHelper.LOGINBEFOREURL];
                    //如果登陆前的地址有，就重定向到登录前的页面
                    if (navUrl != null)
                    {
                        context.Response.Redirect(navUrl);
                    }
                    context.Response.Redirect("~/Index.ashx");//进入系统后台
                    return;
                }
                RPWRazor.RPRazorHelper.OutputRazor(context, "/AdminUser/Login.cshtml");
                //加载完，返回
                return;
            }
            else if ("login" == action)
            {
                string username = context.Request["username"];
                string password = context.Request["password"];
                string autologin = context.Request["autologin"];
                string verificationCode_1 = context.Request["verificationCode"];//类名字和局部变量的名字一致的话，将导致类名字不能使用，所以这里修改了局部变量的名字
                //非空验证
                if (string.IsNullOrWhiteSpace(verificationCode_1))
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

                if (verificationCode_1 != sessionVerificationCode)//(用户输入的验证码==服务器端的验证码)吗？
                {
                    RPCommonHelper.ResetYZM(context);//一旦不一致，就将验证码，进行在服务器端的重置。以防止客户端通过修改js，html等阻止验证码的刷新
                    //只要是客户端正常，那么这里生成的验证码，根本不会使用，这里就是防止黑客专杀的。。。

                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "验证码错误！");
                    return;
                }
                //为了明显的三层，，业务逻辑代码都放在Bll层中 --杨sir说。
                //转到Bll。ext。cs中去写代码。
                //调用Bll层中自己封装的方法
                LoginResult result = new AdminUsersBll().Login(username, password);
                if (result == LoginResult.UserNameNotFound)
                {
                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "用户名不存在！");
                    return;
                }
                else if (result == LoginResult.PasswordError)
                {
                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "密码错误！");
                    return;
                }
                else if (result == LoginResult.UserNameDisabled)
                {
                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "该用户名已经禁止使用！");
                    return;
                }
                else if (result == LoginResult.OK)
                {
                    //如果是登录成功的话，就将用户名和密码，加密放到Session中，加密算法使用Des加密算法；
                    //密码不能是明文保存，否则为危险；
                    //如果自动登录是“on”；
                    if (autologin == "on")
                    {
                        //如果是自动登录勾选的话
                        //保存，
                        //
                        AdminHelper.Remember(context, username, password);

                    }

                    //取得用户的id
                    AdminUsers user = new AdminUsersBll().GetByUserName(username);
                    //记到Session中
                    AdminHelper.StoreInSession(context, user.Id, user.UserName);
                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "登录成功！");

                }
                else
                {
                    throw new Exception("result未知" + result);
                }

            }
            else if ("loginOut" == action)
            {
                //退出登录，逻辑代码还是写在AdminHelper中
                context.Session.Abandon();//销毁Session
                AdminHelper.Forget(context);//销毁Cookie
                context.Response.Redirect("Login.ashx?action=index");//返回到登录界面

            }
            else
            {
                throw new Exception("action错误！");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}