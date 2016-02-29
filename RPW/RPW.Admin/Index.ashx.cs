using RPW.Admin.AdminUser;
using RPWRazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace RPW.Admin
{
    /// <summary>
    /// Index 的摘要说明
    /// </summary>
    public class Index : IHttpHandler, IRequiresSessionState //Session
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            //尝试进入这个页面的时候，检查用户信息是否在Session中存在
            /*
            if (AdminHelper.GetUserIdInSession(context)==null)
            {
                context.Response.Redirect("Login.ashx?action=index");
                return;
                //多次调用，放到AdminHelper中
            }
            */

            AdminHelper.CheckAccess(context);//检查是否登录，是否有权限,同样在AdminUserController.ashx文件也是都要做检查

            string userName= AdminHelper.GetUserNameInSession(context);
            string html = RPRazorHelper.ParseRazor(context, "~/Index.cshtml", new { UserName=userName});
            context.Response.Write(html);
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