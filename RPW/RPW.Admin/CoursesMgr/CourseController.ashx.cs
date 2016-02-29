using RPW.Bll;
using RPW.Model;
using RPWRazor;
using System;
using System.Collections.Generic;
using System.Web;
using RPWCommonts;
using RPW.Admin.AdminUser;
using System.Web.Script.Serialization;

namespace RPW.Admin.CoursesMgr
{
    /// <summary>
    /// Courses 的摘要说明
    /// </summary>
    public class CourseController : BaseHandler
    {
        CoursesBll courBll = new CoursesBll();
        //面向对象中继承的优点：把通用的工作交给父类完成

        //现在，在子类中 开始定义方法 “展示”、“新增”、“编辑”、“保存”、“删除”
        public void list(HttpContext context)
        {
            //课程列表展示
            List<Courses> courses = new CoursesBll().GetModelList("");

            RPRazorHelper.OutputRazor(context, "~/CoursesMgr/CourseList.cshtml", courses);
        }

        public void addnew(HttpContext context)
        {
            //课程新增
            RPRazorHelper.OutputRazor(context, "~/CoursesMgr/CourseAddnewEdit.cshtml",
                new
                {
                    action = "addnew",
                    id = "",
                    courseName = "",
                    serialNo = "",
                    description = ""
                });

        }

        public void edit(HttpContext context)
        {
            string id = context.Request["id"];
            long idCourse = Convert.ToInt64(id);
            var course = courBll.GetModel(idCourse);

            //课程编辑
            RPRazorHelper.OutputRazor(context, "~/CoursesMgr/CourseAddnewEdit.cshtml",
                new
                {
                    action = "edit",
                    id = idCourse,
                    courseName = course.CName,
                    serialNo = course.SerialNo, //用户页面点击保存的时候，确定是编辑的哪一个记录
                    description = course.DescriptionCourses
                });

        }

        public void save(HttpContext context)
        {
            //获取参数
            string saveAction = context.Request["saveAction"];
            string name = context.Request["courseName"];
            string description = context.Request["descrCourse"];
            string serial = context.Request["serialNo"];

            //验证..
            if (string.IsNullOrWhiteSpace(name))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "系统提示\\n，课程名称不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(serial))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "系统提示\\n，课程序号不能为空");
                return;
            }

            if (saveAction == "addnew")
            {
                //课程新增保存
                //存入Model
                Courses course = new Courses();
                course.CreateDateTime = DateTime.Now;
                course.CName = name;
                course.DescriptionCourses = description;
                int serialNo = Convert.ToInt32(serial);
                course.SerialNo = serialNo;

                courBll.Add(course);
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "新增成功");
            }
            else if (saveAction == "edit")
            {
                //获取编辑的那一条
                string id = context.Request["id"];
                long idC = Convert.ToInt64(id);
                var course = courBll.GetModel(idC);
                course.CName = name;
                course.DescriptionCourses = description;
                int serialNo = Convert.ToInt32(serial);
                course.SerialNo = serialNo;

                //课程修改保存
                courBll.Update(course);
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "修改成功");
            }
            else
            {

                throw new Exception("saveAction错误！");
            }


        }

        public void delete(HttpContext context)
        {
            //课程删除
            long id = Convert.ToInt64(context.Request["id"]);
            courBll.Delete(id);
            context.Response.Redirect("CourseController.ashx?action=list");
        }


        public void CreateLearnCards(HttpContext context)
        {
            //todo:检查是否有权限
            //AdminHelper.CheckPower("生成学习卡");

            CoursesBll courseBll = new CoursesBll();
            long[] select = new long[] { 6 }; //数值决定选中那个课程作为第一个值        //因为数组的写法，没有写对，浪费了一个小时，基本功不好。。。。。。。。
            List<Courses> courses = courseBll.GetModelList("");
            RPWRazor.RPRazorHelper.OutputRazor(context, "~/CoursesMgr/CreateLearnCard.cshtml",
                 new { Courses = courses, selecetValue = select });
        }
        //生成学习卡
        public void CreateLearnCardsSubmit(HttpContext context)
        {


            //todo:检查是否有权限
            //AdminHelper.CheckPower("生成学习卡");

            long courseId = Convert.ToInt64(context.Request["courseId"]);
            string prefix = context.Request["prefix"];
            int expireDays = Convert.ToInt32(context.Request["expireDays"]);
            int startNo = Convert.ToInt32(context.Request["startNo"]);
            int endNo = Convert.ToInt32(context.Request["endNo"]);
            //todo:非空验证
            if (string.IsNullOrWhiteSpace(courseId.ToString()))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "课程号不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(prefix))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "学习卡的前缀不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(prefix))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "过期时间不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(prefix))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "开始数字不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(prefix))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "结束数字不能为空");
                return;
            }

            //调用学习卡表的Bll
            LearningCardsBll learnCardBll = new LearningCardsBll();
            //调用学习卡表的Model
            List<LearningCards> cards = new List<LearningCards>();
            //根据课程号，拿到课程名字，因为生成学习卡，都是选中一们
            CoursesBll couseBll = new CoursesBll();
            Courses course = couseBll.GetModel(courseId);
            string couseName = course.CName;
            //执行学习卡表的Bll中的，生成学习卡的方法
            bool isSuccess = learnCardBll.GenerateCards(courseId, prefix, expireDays, startNo, endNo, cards);
            if (isSuccess)
            {

                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", couseName, cards);
            }
            else
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "生成失败，可能是卡号冲突造成的", cards);
            }
        }
    }
}