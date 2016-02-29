using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RPWRazor;
using RPW.Bll;
using RPW.Model;
using RPWCommonts;

namespace RPW.Admin.CoursesMgr
{
    /// <summary>
    /// SegmentController 的摘要说明
    /// </summary>
    public class SegmentController : BaseHandler
    {
        ChaptersBll chapBll = new ChaptersBll();
        SegmentsBll segmBll = new SegmentsBll();

        //段落的 增，改，保存：增、改，删
        public void list(HttpContext context)
        {
            //段落展示
            //获取，chapterid---指定要修改，哪个章节的
            string chapterId = context.Request["chapterId"];
            //存入到Session中，方便在删除的方法中调用
            context.Session["chapterId"] = chapterId;
            //章节的名称
            string chapterName = chapBll.GetModel(Convert.ToInt64(chapterId)).Name;
            //段落列表
            var segments = new SegmentsBll().GetModelList("");
            RPRazorHelper.OutputRazor(context, "~/CoursesMgr/SegmentList.cshtml",
                new { segments = segments, chapterId = chapterId, chapterName = chapterName });

        }
        public void addnew(HttpContext context)
        {
            string chapterId = context.Request["chapterId"];//请求新增页面的时候，拿到章节的id（若不拿，无法加载列表模板）
            //章节的名称
            Chapters chap = chapBll.GetModel(Convert.ToInt64(chapterId));
            RPRazorHelper.OutputRazor(context, "~/CoursesMgr/SegmentAddnewEdit.cshtml",
               new
               {
                   action = "addnew",
                   id = "",
                   segmentName = "",
                   serialNo = "",
                   chapterId = chapterId,//用隐藏字段保存
                   chapterName = chap.Name,
                   label = "新增",
                   videoCode = "",
                   note = ""
               });
        }
        public void edit(HttpContext context)
        {
            string chapterId = context.Request["chapterId"];//请求修改页面的时候，拿到章节的id
            string id = context.Request["id"];//拿到编辑那个段落的id
            long idSegment = Convert.ToInt64(id);
            var segmet = segmBll.GetModel(idSegment);
            //章节的名称
            string chapterName = chapBll.GetModel(Convert.ToInt64(chapterId)).Name;
            //段落编辑
            RPRazorHelper.OutputRazor(context, "~/CoursesMgr/SegmentAddnewEdit.cshtml",
                new
                {
                    action = "edit",
                    id = id,
                    segmentName = segmet.Name,
                    serialNo = segmet.SerialNo, //用户页面点击保存的时候，确定是编辑的哪一个记录
                    chapterId = chapterId,//用隐藏字段保存
                    chapterName = chapterName,
                    label = "",
                    videoCode = segmet.VideoCode,
                    note = segmet.Note
                });

        }
        public void save(HttpContext context)
        {

            //获取参数
            string saveAction = context.Request["saveAction"];
            string name = context.Request["segmetsName"];
            string note = context.Request["note"];
            string videoCode = context.Request["videoCode"];
            string serial = context.Request["serialNo"];
            string chapterId = context.Request["chapterId"];
            //验证..
            if (string.IsNullOrWhiteSpace(name))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "系统提示\n段落名称不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(serial))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "系统提示\n段落序号不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(videoCode))
            {
                RPAjaxhelperCommons.WriteJson(context.Response, "error", "系统提示\n视频代码不能为空");
                return;
            }
            if (saveAction == "addnew")
            {
                //段落保存
                Segments segment = new Segments();
                segment.CreateDateTime = DateTime.Now;
                int serialNo = Convert.ToInt32(serial);
                segment.SerialNo = serialNo;//序号
                segment.Name = name;
                segment.Note = note;
                segment.VideoCode = videoCode;
                segment.ChapterId = Convert.ToInt64(chapterId);//段落属于哪个章节
                //插入
                segmBll.Add(segment);
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "新增成功");
            }
            else if (saveAction == "edit")
            {
                string id = context.Request["id"];
                long idSe = Convert.ToInt64(id);
                var segment = segmBll.GetModel(idSe);
                int serialNum = Convert.ToInt32(serial);
                segment.SerialNo = serialNum;//段落编号
                segment.Name = name;//段落名字
                segment.VideoCode = videoCode;//视频代码
                segment.Note = note;//笔记
                //更新
                segmBll.Update(segment);
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "修改成功");
            }
            else
            {
                throw new Exception("saveAction错误！");
            }


        }
        public void delete(HttpContext context)
        {
            //段落删除
            long id = Convert.ToInt64(context.Request["id"]);
            segmBll.Delete(id);
            //取出，章节号，从session中，否则，在加载模板文件SegmentList的时候，没有chapterId 不能正常加载
            string chapterId = (string)context.Session["chapterId"];
            context.Response.Redirect("SegmentController.ashx?action=list&chapterId=" + chapterId);
        }
    }
}