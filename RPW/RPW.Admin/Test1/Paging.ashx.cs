using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPW.Admin.Test1
{
    /// <summary>
    /// Paging 的摘要说明
    /// </summary>
    public class Paging : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            //获取用户请求的页码
            long pagenum = Convert.ToInt64(context.Request["pagenum"]);

            //一页设定 10 条新闻
            long[] nums = new long[10];  //设置数组的长度为50

            for (int i = 0; i < 10; i++)
            {
                nums[i] = ((pagenum - 1) * 10) + i + 1;
            }
            RPWRazor.RPRazorHelper.OutputRazor(context, "~/Test1/Paging.cshtml", new { nums = nums, pageNum = pagenum });


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