using RPW.Bll;
using RPW.Model;
using RPWCommonts;
using RPWRazor;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace RPW.Admin.AdminUser
{
    /// <summary>
    /// PowerController 的摘要说明
    /// </summary>
    public class PowerController : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //检查是否登录，是否有权限
            AdminHelper.CheckAccess(context);

            context.Response.ContentType = "text/html";//1.0
            string action = context.Request["action"];
            PowersBll bll = new PowersBll();
            if (action == "list")
            {
                //权限列表的展示
                PowersBll powerBll = new PowersBll();
                //将查询的结果放到list集合中
                List<Powers> powers = powerBll.GetModelList("");//不加任何查询条件
                RPRazorHelper.OutputRazor(context, "~/AdminUser/PowerList.cshtml", powers);//将从model中获取的list集合做为model传递
                //到PowerList.cshtml中进行页面展示


            }
            else if (action == "addnew")
            {
                AdminHelper.CheckPower("新增权限");

                //权限的添加,加载新添加的页面    //传action
                RPRazorHelper.OutputRazor(context, "~/AdminUser/PowerAddnewEdit.cshtml", new { powerName = "", action = "addnew", title = "新增", id = "" });//新增时权限名称替换为空,隐藏字段id替换

            }
            else if (action == "edit")
            {
                AdminHelper.CheckPower("编辑权限");
                //权限的修改,同样加载新增页面的模板
                //同时，需要传递 旧的数据到新增页面
                //根据权限的id获取，权限的名称
                long id = Convert.ToInt64(context.Request["id"]);
                Powers power = bll.GetModel(id);
                //使用匿名类                    //传action
                RPRazorHelper.OutputRazor(context, "~/AdminUser/PowerAddnewEdit.cshtml", new { powerName = power.Name, action = "edit", title = "编辑", id = id });
            }
            else if (action == "save")
            {

                string power = context.Request["power"];
                string saveAction = context.Request["saveAction"];

                //因为无论新增还是编辑保存都会进行非空验证，和重复权限名称的验证

                //权限名称为空验证
                if (string.IsNullOrWhiteSpace(power))//1.1.3 
                {
                    RPAjaxhelperCommons.WriteJson(context.Response, "error", "权限名称必填！");
                    return;
                }

                if (saveAction == "edit")
                {
                    AdminHelper.CheckPower("编辑权限");//恶意用户的攻击

                    //获取选中的id，id从模板文件中传递过来,在请求地址栏中添加id信息
                    long id = Convert.ToInt64(context.Request["id"]);
                    //在Bll.Ext.cs中写逻辑判断
                    if (bll.IsUpdae(id, power))//尽量少的代码写在UI层，所以在bll层中做了一个方法处理
                    {

                        #region 记录“修改”权限到日志表中
                        //根据id查出来是修改了哪个权限（旧的权限的名字）
                        string powerName = AdminHelper.PoweridToName(id);
                        AdminHelper.RecordOperationLog("修改权限名称：" + powerName + "为：" + power);
                        #endregion

                        RPAjaxhelperCommons.WriteJson(context.Response, "ok", "权限名称修改成功！");
                    }
                }
                else if (saveAction == "addnew")
                {
                    AdminHelper.CheckPower("新增权限");//恶意用户的攻击

                    //权限是否已经存在验证(---只是在新增的时候进行，“权限名称重复”的验证：总不能，点击修改，就必须修改为不同的名字吧！还是可以用原来的名字，所以这个验证只在新增中做验证就行了)
                    if (bll.IsPowerNameExits(power))
                    {
                        RPAjaxhelperCommons.WriteJson(context.Response, "error", "权限名称已经存在！");
                        return;
                    }
                    bll.AddPower(power);

                    #region 记录“新增”权限到日志表中
                    AdminHelper.RecordOperationLog("新增权限：" + power);
                    #endregion

                    RPAjaxhelperCommons.WriteJson(context.Response, "ok", "权限名称新增成功！");
                }
                else
                {
                    throw new Exception("saveAction错误");
                }
            }
            else if ("delete" == action)
            {
                AdminHelper.CheckPower("删除权限");
                //a.1 获取地址栏递过来的id值
                string id = context.Request["id"];
                //a.2 防止漏洞，进行转换
                long idd = Convert.ToInt64(id);

                #region 记录“删除”权限到日志表中
                string powername = AdminHelper.PoweridToName(idd);
                AdminHelper.RecordOperationLog("删除权限:" + powername);
                #endregion


                //a.3 调用bll层中的delete方法，进行删除
                bll.Delete(idd);
                //a.4 删除成功后，跳转页面到list
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "权限名称删除成功！");
                context.Response.Redirect("PowerController.ashx?action=list");
            }
            else if ("batchDelete" == action)
            {
                //是否有批量删除权限
                AdminHelper.CheckPowerOnBatchButton(context, "批量删除权限");
                //2.0 获取Ajax传递过来的ids
                string ids = context.Request["selectedRoleIds"];// 1,2,3
                if (string.IsNullOrWhiteSpace(ids))
                {
                    RPAjaxhelperCommons.WriteJson(context.Response, "error", "还没有选中任何行！");
                    return;

                }
                //2.1 需要做数组转化处理，目的：防止Sql漏洞攻击
                string[] strs = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); //返回这样的数组{ "1","2","3"}

                //2.3 字符串的数组的长度
                long[] idArray = new long[strs.Length];//bigInt
                for (int i = 0; i < idArray.Length; i++)
                {
                    idArray[i] = Convert.ToInt64(strs[i]);//{3,5,8}
                    //2.4 这是关键步骤，进行了防止漏洞的注入处理
                }

                //2.5 把数组转化为字符串指定间隔符的字符串
                string idList = string.Join(",", idArray);//"3,5,8"

                #region 记录“批量删除”权限名字到日志
                string powerNames = AdminHelper.powerNames(idList);
                AdminHelper.RecordOperationLog("批量删除了权限：" + powerNames);
                #endregion

                //delete from T_Powers where id in (3,5,8);
                //2.6 调用T_Roles表的bll层方法
                bll.DeleteList(idList);
                //2.7 返回消息
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "");//删除成功，不发送消息了，因为客户端就没有处理

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