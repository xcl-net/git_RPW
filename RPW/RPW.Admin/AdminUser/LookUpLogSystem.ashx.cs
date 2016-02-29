using RPW.Bll;
using RPW.Model;
using RPWRazor;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace RPW.Admin.AdminUser
{
    /// <summary>
    /// LookUpLogSystem 的摘要说明
    /// </summary>
    public class LookUpLogSystem : IHttpHandler, IRequiresSessionState //Session，做页面验证的时候，进行加入接口
    {
        public void ProcessRequest(HttpContext context)
        {
            //检查是否登录，是否有权限
            AdminHelper.CheckAccess(context);

            context.Response.ContentType = "text/html";
            string action = context.Request["action"];
            AdministorOperationLogsBll logBll = new AdministorOperationLogsBll();

            List<AdministorOperationLogs> logs = new List<AdministorOperationLogs>(); //按用户名查询时候使用

            //将查询的“关键字”放入到一个list集合中
            //List<KeyWords> keyList = new List<KeyWords>();

            //此处，添加搜索关键字  (不能这样做，在做搜索的时候会有问题，，有id却没办法拿到名字)

            //keyList.Add(new KeyWords { Id = 1, KeyOfSerch = "新增" });
            //keyList.Add(new KeyWords { Id = 2, KeyOfSerch = "批量" });
            //keyList.Add(new KeyWords { Id = 3, KeyOfSerch = "批量修改" });
            //keyList.Add(new KeyWords { Id = 4, KeyOfSerch = "修改" });
            //keyList.Add(new KeyWords { Id = 5, KeyOfSerch = "禁用" });
            //keyList.Add(new KeyWords { Id = 6, KeyOfSerch = "批量禁用" });
            //keyList.Add(new KeyWords { Id = 7, KeyOfSerch = "删除" });
            //keyList.Add(new KeyWords { Id = 8, KeyOfSerch = "批量删除" });




            //查询出来所有的关键字
            List<KeyWords> keyList = new KeyWordsBll().GetModelList("");



            if (action == "loadOption")
            {
                AdminHelper.CheckPower("查询系统操作日志");
                //或者先经集合 logs 设置为 logs.Clear（方法一）
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/AdminUser/LookUpLogSystem.cshtml",
                    new
                    {
                        Uname = "",
                        KeyOfSerchs = keyList,
                        KeyCharcterId = new long[] { 0 },
                        Records = new List<AdministorOperationLogs>(),//（方法二）
                        Alert = RPRazorHelper.Raw("")
                    }

                        );

            }
            else if (action == "serch")//日志搜索功能!! 
            {
                #region 获取用户给的条件 用户名、时间段、关键词
                //获得用户名
                string username = context.Request["uName"];
                //获得时间段
                string dateTimeStart = context.Request["timeStart"];

                string dateTimeFinish = context.Request["timeFinish"];

                //获得关键词id,就是在标签中的value值
                string keyWordValue = context.Request["opeDescription"];

                #endregion

                if (logBll.IsTwoNoNullTime(dateTimeStart, dateTimeFinish))//起始时间和结束时间
                {
                    #region 包含时间段的日志查询（那么，“起始时间”和“结束时间” 都不为空）
                    //拼接时间where条件（功能一：为了做单独的时间查询，功能二：做逻辑条件，判断用户选择产生的情况）
                    string timeWhere = " OperateDate  between  '" + dateTimeStart + " 00:00:00' and  '" + dateTimeFinish + " 23:59:59 '";
                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(timeWhere) && !string.IsNullOrWhiteSpace(keyWordValue))
                    {
                        //获得关键词的具体文本内容
                        long keyWordId = Convert.ToInt64(keyWordValue);
                        string keyTxt = logBll.GetKeyword(keyWordId);

                        //按时间、用户名和关键词（情况：1）
                        logs = logBll.SerchByKeyTimeUserName(dateTimeStart, dateTimeFinish, keyTxt, username);
                        if (logs.Count == 0)
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw("<script language=javascript>alert('系统提示！\\n没有该时间段:" + dateTimeStart + "--" + dateTimeFinish + "，并且用户名为：" + username + "，关键词为：" + keyTxt + ",的日志记录！')</script>"), keyList, logs);
                        }
                        else
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw(""), keyList, logs);//查出来数据，不提示
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(timeWhere))
                    {
                        //按时间、用户名（情况：2）
                        long Userid = logBll.GetUserNameId(username);
                        logs = logBll.SearchByNameTime(dateTimeStart, dateTimeFinish, Userid);
                        if (logs.Count == 0)
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw("<script language=javascript>alert('系统提示！\\n没有该时间段:" + dateTimeStart + "--" + dateTimeFinish + "，并且用户名为：" + username + "，的日志记录！')</script>"), keyList, logs);
                        }
                        else
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw(""), keyList, logs);//查出来数据，不提示
                        }

                    }
                    else if (!string.IsNullOrWhiteSpace(keyWordValue) && !string.IsNullOrWhiteSpace(timeWhere))
                    {
                        //按时间、关键词（情况：4）

                        //获得关键词的具体文本内容
                        long keyWordId = Convert.ToInt64(keyWordValue);
                        string keyTxt = logBll.GetKeyword(keyWordId);

                        logs = logBll.SearchByKeywordTime(dateTimeStart, dateTimeFinish, keyTxt);
                        //查询结束，页面展示
                        if (logs.Count == 0)
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw("<script language=javascript>alert('系统提示！\\n没有该时间段:" + dateTimeStart + "--" + dateTimeFinish + "，并且关键词为：" + keyTxt + "，的日志记录！')</script>"), keyList, logs);
                        }
                        else
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw(""), keyList, logs);//查出来数据，不提示
                        }


                    }
                    else if (!string.IsNullOrWhiteSpace(timeWhere))
                    {
                        //按时间（情况：6）
                        logs = logBll.SearchByTime(timeWhere);
                        if (logs.Count == 0)
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw("<script language=javascript>alert('系统提示！\\n没有该时间段:" + dateTimeStart + "--" + dateTimeFinish + "的日志记录！')</script>"), keyList, logs);
                        }
                        else
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw(""), keyList, logs);//查出来数据，不提示
                        }
                    }
                    #endregion
                    else
                    {
                        context.Response.Write("查询条件未知！");
                        return;
                    }
                }
                else
                {
                    #region 不包含时间段的日志查询

                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(keyWordValue))
                    {
                        //按用户名、关键词（情况：3）

                        //获得关键词的具体文本内容
                        long keyWordId = Convert.ToInt64(keyWordValue);
                        string keyTxt = logBll.GetKeyword(keyWordId);
                        logs = logBll.SerchByUserKEY(username, keyTxt);
                        //加载模板
                        if (logs.Count == 0)
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw("<script language=javascript>alert('系统提示！\\n查找的用户名:" + username + ",不存在，或着没有该关键词:" + keyTxt + ",的记录！')</script>"), keyList, logs);
                        }
                        else
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw(""), keyList, logs);//查出来数据，不提示
                        }
                    }

                    else if (!string.IsNullOrWhiteSpace(username))
                    {
                        //按用户名（情况：5）
                        logs = logBll.SearchByUsername(username);
                        if (logs.Count == 0)//集合验证用户 count 属性
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw("<script language=javascript>alert('系统提示！\\n查找的用户名:" + username + ",不存在，该用户日志为空！')</script>"), keyList, logs);
                        }
                        else
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw(""), keyList, logs);//查出来数据，不提示
                        }

                    }

                    else if (!string.IsNullOrWhiteSpace(keyWordValue))
                    {
                        //按关键词（情况：7）

                        //获得关键词的具体文本内容
                        long keyWordId = Convert.ToInt64(keyWordValue);
                        string keyTxt = logBll.GetKeyword(keyWordId);

                        logs = logBll.SearchByKeyWord(keyTxt);
                        if (logs.Count == 0)
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw("<script language=javascript>alert('系统提示！\\n有关该关键词:" + keyTxt + ",日志为空！')</script>"), keyList, logs);
                        }
                        else
                        {
                            AdminHelper.LoadCshtml(context, RPRazorHelper.Raw(""), keyList, logs);//查出来数据，不提示
                        }
                    }
                    #endregion
                    else
                    {
                        context.Response.Write("查询条件未知！");
                        return;
                    }
                }
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