using RPW.Bll;
using RPW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPW.Front
{
    /// <summary>
    /// ViewCourses 的摘要说明
    /// </summary>
    public class ViewCourses : BaseHandler
    {

        public void LookCourses(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            // 点击课程的id后，就显示这个课程的章节列表信息

            //拿到课程的id
            long courseId = Convert.ToInt64(context.Request["courseId"]);
            //由 课程的id 判断是否，存在这个课程
            Courses course = new CoursesBll().GetModelByCache(courseId);
            if (course == null)
            {
                //调用错误输出方法
                FrontHelper.OutPutError(context, "章节不存在！");
                return;
            }
            long? userId = FrontHelper.GetUserIdInSession(context);
            if (userId == null)
            {
                context.Response.Redirect("/login.shtml");
                return;
            }
            //检查登录的这个用户id是否购买了，这个课程
            UserCoursesBll bll = new UserCoursesBll();
            //检查是否已经购买了，这个课程
            if (bll.CheckHasCourse(courseId, (long)userId))
            {
                //章节的列表
                List<Chapters> chapters = new ChaptersBll().GetModelListByCache("CourseId=" + courseId);
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/ViewCourses.cshtml", new { course = course, chapters = chapters, buyInfo = "" });//把两个对象，都传递到模板中

            }
            else
            {
                //章节的列表
                List<Chapters> chapters = new ChaptersBll().GetModelListByCache("CourseId=" + courseId);
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/ViewCourses.cshtml", new { course = course, chapters = chapters, buyInfo = "购买本课程" });//把两个对象，都传递到模板中

            }


        }
    }
}