using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RPW.Bll;
using RPW.Model;
using RPWCommonts;
using System.Data.SqlClient;

namespace RPW.Admin.LookUpLogs
{
    /// <summary>
    /// AdminOperaLogController 的摘要说明
    /// </summary>
    public class AdminOperaLogController : BaseHandler
    {
        //加载日志查看页面
        public void LoadLogsPage(HttpContext context)
        {
            RPWRazor.RPRazorHelper.OutputRazor(context, "/LookUpLogs/AdminOperaLogSearch.cshtml");
        }

        //点击“搜索”按钮后，展示全部的日志信息
        public void SerchAllLogs(HttpContext context)
        {
            /*
            //遍历出所有的日志
            AdministorOperationLogsBll logBll = new AdministorOperationLogsBll();
            List<AdminOpreResultLogs> logModels = logBll.GetModelListByAjax();
            //因为日期时间类型，通过json会，乱码，所以在ui层需要做特殊处理
            //声明一个list集合接收，特殊处理过的日志记录
            List<object> list = new List<object>();   //注意：这里是使用的一个object类型，用来接收匿名类
            foreach (AdminOpreResultLogs log in logModels)
            {
                //匿名类，也是 视图模式
                list.Add(new
                {
                    name = log.Name,
                    time = log.OperaTime.ToString(),
                    opre = log.Description
                });

            }
             */




            //实例化一个条件model，接收用户选择的查询条件
            AdminOperationLogSearchOption option = new AdminOperationLogSearchOption();

            //按用户名
            if (context.Request["cbByUserName"] == "on")
            {
                option.SearchByUserName = true;//设置为Ture，这样dal层，就会附加上这个条件
                option.UserName = context.Request["name"];
            }

            //按时间
            if (context.Request["cbByOpDateTime"] == "on")
            {
                option.SearchByCreateDateTime = true;
                //开始时间
                string startTime = context.Request["timeStart"];
                option.StartTime = DateTime.Parse(startTime);
                //结束时间
                string endTime = context.Request["timeEnd"];
                option.EndTime = DateTime.Parse(endTime);

            }

            //按关键字
            if (context.Request["cbByDesc"] == "on")
            {
                option.SearchByDesc = true;
                option.Description = context.Request["desc"];

            }

            //调用Bll层，查询出结果
            AdministorOperationLogsBll logBll = new AdministorOperationLogsBll();
            List<AdminOpreResultLogs> logModels = logBll.GetModelListByAjax_option(option);

            //因为日期时间类型，通过json会，乱码，所以在ui层需要做特殊处理
            //声明一个list集合接收，特殊处理过的日志记录
            List<object> list = new List<object>();   //注意：这里是使用的一个object类型，用来接收匿名类
            foreach (AdminOpreResultLogs log in logModels)
            {
                //匿名类，也是 视图模式
                list.Add(new
                {
                    name = log.Name,
                    time = log.OperaTime.ToString(),
                    opre = log.Description
                });

            }

            //传到浏览器，Json
            RPAjaxhelperCommons.WriteJson(context.Response, "ok", "", list);
        }


    }
}