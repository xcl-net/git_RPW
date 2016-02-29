using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace RPW.Admin.AdminUser
{
    /// <summary>
    /// verificationCode 的摘要说明
    /// </summary>
    public class verificationCode : IHttpHandler, IRequiresSessionState//   实现接口，shift+alt+f10导入命名
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/jpeg";//1，修改报文输出的类型
            //生成验证码
            RPWCommonts.RPCommonHelper.GenerateValidCode(context);

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