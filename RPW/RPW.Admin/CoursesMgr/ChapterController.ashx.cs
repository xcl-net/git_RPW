using RPW.Bll;
using RPW.Model;
using RPWCommonts;
using RPWRazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPW.Admin.CoursesMgr
{
    /// <summary>
    /// ChapterController 的摘要说明
    /// </summary>
    public class ChapterController : BaseHandler
    {
        ChaptersBll chapBll = new ChaptersBll();
        CoursesBll courBll = new CoursesBll();

        //章节 新增、编辑、保存:新增，编辑、删除
        public void list(HttpContext context)
        {
            //章节列表展示

            //获取，coursesId---指定要修改，哪个课程的：章节管理
            string coursesId = context.Request["coursesId"];
            //存入到Session中，方便删除调用
            context.Session["courseId"] = coursesId;
            //课程的名称
            Courses cour = courBll.GetModel(Convert.ToInt64(coursesId));
            //获取章节列表
            List<Chapters> chapters = new ChaptersBll().GetModelList("");

            RPRazorHelper.OutputRazor(context, "~/CoursesMgr/ChapterList.cshtml",
             new { chapters = chapters, coursesId = coursesId, courseName = cour.CName });
        }
        public void addnew(HttpContext context)
        {
            string coursesId = context.Request["id"];//请求新增页面的时候，拿到课程的id（若不拿，无法加载列表模板）
            //课程的名称
            Courses cour = courBll.GetModel(Convert.ToInt64(coursesId));
            //章节新增
            RPRazorHelper.OutputRazor(context, "~/CoursesMgr/ChapterAddnewEdit.cshtml",
                new
                {
                    action = "addnew",
                    id = "",
                    chapterName = "",
                    serialNo = "",
                    description = "",
                    coursesId = coursesId,//用隐藏字段保存
                    courseName = cour.CName,
                    label = "新增"
                });
        }

        public void edit(HttpContext context)
        {
            string coursesId = context.Request["courseIdd"];//请求修改页面的时候，拿到课程的id（若不拿，无法加载列表模板）
            string id = context.Request["id"];
            long idChapter = Convert.ToInt64(id);
            var chapter = chapBll.GetModel(idChapter);

            //课程的名称
            Courses cour = courBll.GetModel(Convert.ToInt64(coursesId));

            //章节编辑
            RPRazorHelper.OutputRazor(context, "~/CoursesMgr/ChapterAddnewEdit.cshtml",
                new
                {
                    action = "edit",
                    id = idChapter,
                    chapterName = chapter.Name,
                    serialNo = chapter.SerialNo, //用户页面点击保存的时候，确定是编辑的哪一个记录
                    description = chapter.DescriptionChapter,
                    coursesId = coursesId,//用隐藏字段保存
                    courseName = cour.CName,
                    label = ""
                });
        }

        public void save(HttpContext context)
        {
            //获取参数
            string saveAction = context.Request["saveAction"];
            string name = context.Request["chapterName"];
            string description = context.Request["descrChapter"];
            string serial = context.Request["serialNo"];
            string coursesId = context.Request["coursesId"];

            //验证..
            if (string.IsNullOrWhiteSpace(name))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "系统提示\n章节名称不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(serial))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "系统提示\n章节序号不能为空");
                return;
            }

            if (saveAction == "addnew")
            {
                //章节保存
                Chapters chapter = new Chapters();
                chapter.CreateDateTime = DateTime.Now;//时间
                int serialNo = Convert.ToInt32(serial);
                chapter.SerialNo = serialNo;//序号
                chapter.Name = name;//章节名
                chapter.DescriptionChapter = description;//章节描述
                chapter.CourseId = Convert.ToInt64(coursesId);//章节属于哪个课程  （多对一的关系，多对多，这里就不需要了）
                //插入
                chapBll.Add(chapter);
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "新增成功");
            }
            else if (saveAction == "edit")
            {
                //获取编辑的那一条
                string id = context.Request["id"];
                long idChap = Convert.ToInt64(id);
                var chapter = chapBll.GetModel(idChap);
                int serialNo = Convert.ToInt32(serial);
                chapter.SerialNo = serialNo;//序号
                chapter.Name = name;//章节名
                chapter.DescriptionChapter = description;//章节描述
                //更新
                chapBll.Update(chapter);
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "修改成功");
            }
            else
            {

                throw new Exception("saveAction错误！");
            }

        }

        public void delete(HttpContext context)
        {
            //章节删除
            long id = Convert.ToInt64(context.Request["id"]);
            chapBll.Delete(id);
            //取出，课程号，从session中，否则，在加载模板文件ChapterList的时候，没有coursesId 不能正常加载
            string coursesId = (string)context.Session["courseId"];
            context.Response.Redirect("ChapterController.ashx?action=list&coursesId=" + coursesId);
        }



    }
}