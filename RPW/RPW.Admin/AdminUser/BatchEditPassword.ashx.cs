using System.Web;
using System.Web.SessionState;

namespace RPW.Admin.AdminUser
{
    /// <summary>
    /// BatchEditPassword 的摘要说明
    /// </summary>
    public class BatchEditPassword : IHttpHandler, IRequiresSessionState//因为使用了session这个对象，需要该接口
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            //从session中获取，存入的用户列表
            string userList = (string)context.Session["userList"];

            //是否有权限
            AdminHelper.CheckPower("批量重置管理用户密码");

            //加载模板“重置密码”页面
            RPWRazor.RPRazorHelper.OutputRazor(context, "~/AdminUser/BatchEditPassword.cshtml", new { userList = userList });

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