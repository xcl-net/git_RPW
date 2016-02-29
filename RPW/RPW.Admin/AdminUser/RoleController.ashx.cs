using RPW.Bll;//
using RPW.Model;
using RPWCommonts;
using RPWRazor;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;//

namespace RPW.Admin.AdminUser
{
    /// <summary>
    /// RoleController 的摘要说明
    /// </summary>
    public class RoleController : IHttpHandler, IRequiresSessionState//因为使用了session这个对象，需要该接口
    {

        public void ProcessRequest(HttpContext context)
        {
            //检查是否登录，是否有权限
            AdminHelper.CheckAccess(context);

            context.Response.ContentType = "text/html";
            string action = context.Request["action"];
            if (action == "list")
            {
                RolesBll roleBll = new RolesBll();
                List<Roles> roles = roleBll.GetModelList("");
                RPRazorHelper.OutputRazor(context, "~/AdminUser/RoleList.cshtml", roles);

            }
            else if (action == "addnew")
            {
                //将权限表中所有的权限都展示在角色新增页面中
                PowersBll powerBll = new PowersBll();
                //将power对象放入list集合中
                List<RPW.Model.Powers> powers = powerBll.GetModelList("");

                AdminHelper.CheckPower("新增角色");

                //使用Razor模板引擎
                RPWRazor.RPRazorHelper.OutputRazor(context,
                    "~/AdminUser/RoleAddnewEdit.cshtml",
                new
                {
                    action = "addnew",
                    powers = powers,
                    id = "",
                    roleName = "",
                    //因为“新增”角色，没有选中任何的权限，所以，选择的ids设置为一个长度为0的数组；
                    selectedPowers = new long[0] { }//选中的项都放在一个long数组中传递到模板文件

                });//id、roleName属性是为隐藏字段用的
            }
            else if (action == "edit")
            {
                //获取要编辑那一项的id
                long id = Convert.ToInt64(context.Request["id"]);
                //拿到id,传递到Dal层，从数据库中查询出来该角色，并且存入到model中
                Roles role = new RolesBll().GetModel(id);
                //
                if (role == null)
                {
                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "id不存在！");
                    return;
                }
                //因为要使用RoleAddnewEdit.cshtml模板文件，故将权限列表进行展示出来
                //调用权限列表的逻辑层Bll
                PowersBll powerBll = new PowersBll();
                List<RPW.Model.Powers> powers = powerBll.GetModelList("");

                //获取给定id角色拥有的权限
                //调用角色权限表的Bll层
                RolePowesBll rolePowerBll = new RolePowesBll();
                List<RolePowes> selectedRolePowers = rolePowerBll.GetModelList("RoleId=" + id);

                //....一个角色“选中的”权限的id的集合
                List<long> selectedPowers = new List<long>();
                //遍历一个角色“选中的”权限的Id，添加到selectedPowers集合中
                foreach (RolePowes rolePower in selectedRolePowers)
                {
                    selectedPowers.Add(rolePower.PowerId);
                }

                AdminHelper.CheckPower("编辑角色");
                //调用模板，并替换
                RPRazorHelper.OutputRazor(context, "~/AdminUser/RoleAddnewEdit.cshtml",
                    new
                    {
                        action = "edit",
                        id = id,
                        roleName = role.Name,//选中的角色的名字，从model中取出，替换模板中的“占位符”
                        powers = powers,
                        selectedPowers = selectedPowers
                    });
            }
            else if (action == "save")
            {
                AdminHelper.CheckPower("新增角色");//防止恶意用户的攻击
                //来到新增保存
                string saveAction = context.Request["saveAction"];
                string name = context.Request["roleName"];
                if (saveAction == "addnew")
                {
                    AdminHelper.CheckPower("新增角色");//恶意用户攻击

                    #region 获得新增角色的名称
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "角色名称必填！");
                        return;
                    }
                    #endregion

                    #region 新增角色，同时使用Add方法获得了，获得新增角色的id
                    //此时用的是T_Roles这张表，调用这张角色表的Bll层
                    RolesBll roleBll = new RolesBll();
                    Roles role = new Roles();
                    role.Name = name;
                    //新增一个角色。add方法返回值是新增角色的id
                    long roleId = roleBll.Add(role);
                    #endregion

                    #region jQuery批量获取角色取选中的“权限”，转化为字符串,使用ajax传递到服务器端
                    string selectedPowerIds = context.Request["selectedPowerIds"];
                    //验证；

                    if (string.IsNullOrWhiteSpace(selectedPowerIds))
                    {
                        RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "您还没有选中任何权限！");
                        return;
                    }
                    #endregion

                    #region 将批量获取的“权限”，转化为long[]数组
                    string[] strs = selectedPowerIds.Split(',');
                    long[] powerIds = new long[strs.Length];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        powerIds[i] = Convert.ToInt64(strs[i]);
                    }
                    #endregion

                    #region 使用业务逻辑层，使用动软的添加方法，循环新增“角色--权限”（角色id不变，权限id随循环递增） 对应关系，
                    RolePowesBll rolePowerBll = new RolePowesBll();
                    rolePowerBll.AddRolePowers(roleId, powerIds);
                    #endregion

                    #region 记录“新增”角色到日志
                    AdminHelper.RecordOperationLog("新增角色：" + name);
                    #endregion

                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "角色权限对应关系成功建到了T_RolePowers表！");
                }
                else if (saveAction == "edit")
                {
                    AdminHelper.CheckPower("编辑角色");//恶意用户的攻击
                    #region 获取角色的id（直接从请求的地址栏获得）（获取这个id，主要是方便T_RolePowes表角色的删除，再做该表“角色--权限”的重新添加，就实现了“更新功能”）
                    long roleId = Convert.ToInt64(context.Request["id"]);
                    #endregion

                    #region 将批量获取的“权限”，转化为long[]数组
                    string selectedPowerIds = context.Request["selectedPowerIds"];
                    string[] strs = selectedPowerIds.Split(',');
                    long[] powerIds = new long[strs.Length];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        powerIds[i] = Convert.ToInt64(strs[i]);
                    }
                    #endregion

                    #region T_RolePowes表角色的删除
                    RolePowesBll rolePowerBll = new RolePowesBll();
                    rolePowerBll.ClearRole(roleId);
                    #endregion

                    #region 角色的名称修改（前）（后），方便记录日志主要是
                    //获取修改角色的名称前的角色名称
                    string rolename = AdminHelper.RoleIdToName(roleId);
                    //获取修改角色的名称后的角色名称
                    Roles role = new RolesBll().GetModel(roleId);
                    role.Name = name;
                    new RolesBll().Update(role);
                    #endregion

                    #region T_RolePowers表“角色--权限”逐条记录（角色的Id没有变化，引用的还是T_Roles表中的原来的角色id）
                    rolePowerBll.AddRolePowers(roleId, powerIds);
                    #endregion

                    #region 记录“修改”角色到日志

                    //获取角色的选中权限的名称
                    string powernames = AdminHelper.powerNames(selectedPowerIds);
                    //日志
                    AdminHelper.RecordOperationLog("修改角色：“" + rolename + "”，名称为：“" + name + "”，并且重新分配权限为：" + powernames);
                    #endregion

                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "角色权限更新成功！");
                }
                else
                {
                    throw new Exception("saveAction错误");
                }
            }
            else if (action == "delete")
            {
                AdminHelper.CheckPower("删除角色");
                #region 删除T_RolePowes表中的角色(先删除，引用的表中的角色)
                //直接用T_RolePowes表的Bll层中的方法
                //获取删除的角色id
                long roleId = Convert.ToInt64(context.Request["id"]);
                #region 若是删除角色，首先，删除 用户--角色 表中的角色id
                AdminUserRolesBll userRoleBll = new AdminUserRolesBll();
                userRoleBll.ClearRolesId(roleId);
                #endregion
                RolePowesBll rolePowerBll = new RolePowesBll();
                rolePowerBll.ClearRole(roleId);
                #endregion

                #region 创建对象，角色表的Bll（再删除被引用的表中的角色）
                RolesBll roleBll = new RolesBll();
                #endregion

                #region 记录“删除”角色到日志
                string rolename = AdminHelper.RoleIdToName(roleId);
                AdminHelper.RecordOperationLog("删除角色：“" + rolename + "”");
                #endregion
                roleBll.Delete(roleId);



                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "角色删除成功！");
                context.Response.Redirect("RoleController.ashx?action=list");
            }
            else if ("batchDelete" == action)
            {
                //是否有批量删除的权限
                AdminHelper.CheckPowerOnBatchButton(context, "批量删除角色");

                //2.0 获取Ajax传递过来的ids
                string ids = context.Request["selectedRoleIds"];// 1,2,3
                if (string.IsNullOrWhiteSpace(ids))
                {
                    RPAjaxhelperCommons.WriteJson(context.Response, "error", "没有选中任何行！");
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
                string roleIdList = string.Join(",", idArray);//"3,5,8"
                //delete from T_Powers where id in (3,5,8);
                //2.6 调用bll层方法 

                //(先删除，引用的表中的角色)<用户--角色表>(第一个引用表)
                AdminUserRolesBll userRoleBll = new AdminUserRolesBll();
                userRoleBll.batchDeleteRoleIDList(roleIdList);

                //（再删除，引用表中的角色）<角色--权限表>(第二个引用表)
                RolePowesBll rolePowerBll = new RolePowesBll();
                rolePowerBll.batchDeleteRoleList(roleIdList);//动软没有提供，删除批量角色的Id，这个自己添加
                //（再删除被引用的表中的角色）（原表）
                RolesBll roleBll = new RolesBll();


                #region 记录“批量删除”角色到日志
                //批量删除的角色名称
                string strWhere = "Id in (" + roleIdList + ")";
                string roleNames = AdminHelper.RoleIdsToNames(strWhere);
                //日志
                AdminHelper.RecordOperationLog("批量删除角色：" + roleNames);
                #endregion

                roleBll.DeleteList(roleIdList);


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