using RPW.Admin.AdminUser;
using System;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace RPW.Admin
{
    public class BaseHandler : IHttpHandler, IRequiresSessionState
    {

        public bool IsReusable
        {
            get { throw new NotImplementedException(); }
        }

        public void ProcessRequest(HttpContext context)
        {
            //url存入到session中
            //拿到url
            string urlList = context.Request.Url.ToString();
            context.Session[AdminHelper.LOGINBEFOREURL] = urlList;


            //1.检查 用户已经登录
            AdminHelper.CheckAccess(context);
            //约定，请求的地址栏中，一定要有一个参数是 action 
            string action = context.Request["action"];
            //获取当前类的 类名字（反射）
            Type contrlerType = this.GetType();
            //根据指定的action的值 拿到 指定的 方法
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new Exception("action不能为空");

            }
            MethodInfo methAction = contrlerType.GetMethod(action);

            //也有可能action的值，是在子类中，没有这个方法：比如：...?action=list123
            //那么，判断：methAction 是 null
            if (methAction == null)
            {
                throw new Exception("action不存在");
            }
            //存在，就开始调用这个：子类中的方法
            methAction.Invoke(this, new object[] { context });//.哪个对象上调用方法，这个方法需要的参数
            //当在派生类中重写时，调用具有给定参数的反射的方法或构造函数。
        }
    }
}