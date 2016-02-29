using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RPW.Bll;
using RPW.Model;
using RPWRazor;


namespace RPW.Front
{
    /// <summary>
    /// ViewSegment 的摘要说明
    /// </summary>
    public class ViewSegment : BaseHandler
    {

        public void LookSegment(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            long id = Convert.ToInt64(context.Request["id"]);
            var segment = new SegmentsBll().GetModel(id);
            if (segment == null)
            {
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/Error.cshtml", "段落不存在");
                return;
            }
            //如果本节课程存在
            //记录日志信息
            //T_WhoLearning
            WhoLernings who = new WhoLernings();
            WhoLerningsBll whoBll = new WhoLerningsBll();

            //当前用户名
            string name = FrontHelper.GetUserNameInSession(context);
            //根据用户名，查出其所在的学校
            FrontUsers frontUser = new FrontUsersBll().GetModelByUserName(name);
            string schoolName = frontUser.SchoolName;//这里点的是学校
            who.Name = name;
            who.LearningTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            who.SchoolName = schoolName;
            who.SegmentName = segment.Name;

            //执行插入记录表
            whoBll.Add(who);

            RPWRazor.RPRazorHelper.OutputRazor(context, "~/ViewSegment.cshtml", segment);

        }


    }
}