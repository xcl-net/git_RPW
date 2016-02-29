using RPWCommonts;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace RPW.Front
{
    public class Global : System.Web.HttpApplication
    {
        public override void Init() //继承父类HttpApplication中的方法
        {
            base.Init();
            // 1.在下次再有浏览器打开这个实现IRequiresSessionState接口的程序。
            //2.都会经过这个监听函数Init( );
            //3.接着触发事件，AcquireRequesState;
            this.AcquireRequestState += Global_AcquireRequestState;
        }

        void Global_AcquireRequestState(object sender, EventArgs e)
        {
            //session为空
            try  //防止出现异常sessino
            {
                if (HttpContext.Current.Session == null)
                {
                    return;
                }
            }
            catch (Exception)
            {

                return;
            }

            string username = FrontHelper.GetUserNameInSession(Context);
            if (username == null)
            {
                return;
            }
            //否子，读取redis中的sessionId
            using (IRedisClient client = RedisManager.ClientManager.GetClient())
            {
                string redisSessionId = client.Get<string>(FrontHelper.FRONTSESSIONID_PREFIX + username);
                //如果redis中，当前redis的SessionId不能与当期请求的Session的值相同，则销毁session信息
                if (redisSessionId != null && redisSessionId != Session.SessionID)
                {
                    //
                    Session.Clear();//清除数据
                    Session.Abandon();//销毁session
                    FrontHelper.Forget(Context);//销毁Cookie,否则，会进行自动登录，自动登录也会把当期那sessionId存到redis中的

                    Context.Response.Redirect("login.shtml");//返回到登录界面
                }

            }
        }
        void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //只要有请求过来，不管地址是否存在，都可以在这里截取到
            //现在我想使用这样的地址
            //Course1.ashx
            string requestUrl = Context.Request.Url.ToString();
            Match matchCourse = Regex.Match(requestUrl, @"Course(\d+)\.ashx");
            if (matchCourse.Success)
            {
                //匹配成功，获取用户要访问的课程id
                string courseId = matchCourse.Groups[1].Value;
                //传递到服务器的地址中
                Context.RewritePath("ViewCourses.ashx?action=LookCourses&courseId=" + courseId);

            }

            //同样，段落进行，修改
            //Segment1.ashx
            Match matchSegment = Regex.Match(requestUrl, @"Segment(\d+)\.ashx");
            if (matchSegment.Success)
            {
                //匹配成功，获取用户要访问的课程id
                string segmentId = matchSegment.Groups[1].Value;
                //传递到服务器的地址中
                Context.RewritePath("ViewSegment.ashx?action=LookSegment&id=" + segmentId);

            }

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}