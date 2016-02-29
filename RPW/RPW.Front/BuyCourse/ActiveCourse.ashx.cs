using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RPW.Bll;
using RPW.Model;

namespace RPW.Front.BuyCourse
{
    /// <summary>
    /// ActiveCourse 的摘要说明
    /// </summary>
    public class ActiveCourse : BaseHandler
    {
        public void actveCourse(HttpContext context)
        {

            string learnCardNum = context.Request["learnCardNum"];
            string learnCarPass = context.Request["learnCarPass"];
            string validCode = context.Request["validCode"];
            //验证
            if (string.IsNullOrWhiteSpace(learnCardNum))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "学习卡号不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(learnCarPass))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "学习卡密码不能为空");
                return;
            }
            if (string.IsNullOrWhiteSpace(validCode))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "验证码不能为空");
                return;
            }
            if (validCode != RPWCommonts.RPCommonHelper.GetValidCode(context))
            {

                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "验证码错误！");
                return;
            }

            //从session中获得当前用户的id
            long userId = (long)FrontHelper.GetUserIdInSession(context);
            //开始激活学习卡
            LearningCardsBll cardBll = new LearningCardsBll();

            //todo:如果没有登录，不能进入到这个页面
            if (string.IsNullOrWhiteSpace(userId.ToString()))
            {
                context.Response.Redirect("login.shtml");
                return;
            }

            //卡号，卡号密码，用户id
            CardAciveResult cardAciveResult = cardBll.Active(learnCardNum, learnCarPass, userId);
            //通过卡号，查出当前学习卡对用的model
            LearningCards card = cardBll.GetModelByCardNum(learnCardNum);
            //激活的结果
            if (cardAciveResult == CardAciveResult.OK)//激活成功
            {

                card.IsActive = true;//激活成功，就将这个卡号的状态码设置为true
                cardBll.Update(card);//更新当前学习卡
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "学习卡成功激活！");
                return;
            }
            else if (cardAciveResult == CardAciveResult.ErrorPassWord)
            {

                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "学习卡密码错误！");
                return;
            }
            else if (cardAciveResult == CardAciveResult.CardNotFound)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "不存在这张学习卡！");
                return;

            }
            else if (cardAciveResult == CardAciveResult.CardHasActived)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "学习已经激活，不需要重复激活！");
                return;
            }
        }



        public void MycourseOfBuy(HttpContext context)
        {
            //todo:如果没有登录，不能进入到这个页面

            //进入到这个页面，根据用户id，检查用户已经购买了哪些课程
            long FrontUseId = (long)FrontHelper.GetUserIdInSession(context);
            if (string.IsNullOrWhiteSpace(FrontUseId.ToString()))
            {
                context.Response.Redirect("login.shtml");
                return;
            }

            //写个方法，检验当前用户，有哪些已经激活的课程
            UserCoursesBll bll = new UserCoursesBll();
            List<UserCourses> userCourses = bll.GetUserCourses(FrontUseId);//用户学习卡，每条记录都添加在list集合中
            //激活的课程的名称是
            RPWRazor.RPRazorHelper.OutputRazor(context, "~/BuyCourse/MyCourse.cshtml",
                new { userCourses = userCourses });
        }



    }
}