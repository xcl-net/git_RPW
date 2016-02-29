using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace RPW.Admin
{
    /// <summary>
    /// Test1 的摘要说明
    /// </summary>
    public class Test2 : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            
            ILog log = LogManager.GetLogger(typeof(Test2));//当前类的名字
            log.Debug("这是调试信息");
            log.Warn("这是警告信息");
            log.Error("这是错误信息");
            log.Fatal("这是严重错误信息");
            context.Response.Write("日志信息就录了");

           
            try
            {
                WebClient wc = new WebClient();//用于请求一个网页的html字符串
                log.Debug("开始下载");
                string s = wc.DownloadString("http://www.qq123123.com");//故意写错，在日志中可看到是这里出了问题
                log.Debug("下载内容为"+s);


            }
            catch (Exception ex)
            {

                log.Error("下载失败", ex);//Error方法重载，打印出来错误堆栈
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