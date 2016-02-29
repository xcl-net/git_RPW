using RPW.Bll;
using RPW.Model;
using RPWCommonts;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;


namespace RPW.Admin.AdminUser
{
    /// <summary>
    /// AdminUserController 的摘要说明
    /// </summary>
    public class AdminUserController : IHttpHandler, IRequiresSessionState //Session，做页面验证的时候，进行加入接口
    {

        public void ProcessRequest(HttpContext context)
        {
            //url存入到session中
            //拿到url
            string urlList = context.Request.Url.ToString();
            context.Session[AdminHelper.LOGINBEFOREURL] = urlList;

            context.Response.ContentType = "text/html";

            //每一个action都是一个登录到后台的页面，因此，每个页面都是需要做session验证
            //所以在这里统一做验证
            //检查是否登录，是否有权限
            AdminHelper.CheckAccess(context);

            //1.1 获取action的信息
            string action = context.Request["action"];
            //1.16  实例化，业务逻辑层对象
            AdminUsersBll bll = new AdminUsersBll();
            if ("list" == action)//显示所有的用户信息
            {
                //1.2 把model类，放到集合中，
                List<AdminUsers> users = new AdminUsersBll().GetModelList("");//这里赋值为""，表示where条件为空字符串
                /*
                string html = RPWRazor.RPRazorHelper.ParseRazor(context, "~/AdminUser/AdminUserList.cshtml", users);
                context.Response.Write(html);
                */
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/AdminUser/AdminUserList.cshtml", users);
            }
            else if ("addnew" == action)
            {
                //1.1.1.1 任务：修改后台用户新增界面;
                //步骤：
                //查出来所有“角色”
                RolesBll roleBll = new RolesBll();
                List<Roles> roles = roleBll.GetModelList("");
                //绑定到checkbox 标签中，使用匿名类
                AdminHelper.CheckPower("新增管理用户");
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/AdminUser/AdminUserAddNew.cshtml",
                    new
                    {
                        action = "addnew",
                        roles = roles,
                        id = "",
                        userName = "",
                        passWord = "",
                        selectedRoles = new long[0] { },//添加用户
                        lable = "当前用户可以选择的角色："
                    });
            }
            else if ("edit" == action)
            {
                //1.1.1.1 任务：修改后台用户编辑界面;
                //步骤：
                //要“修改”那个用户
                long id = Convert.ToInt64(context.Request["id"]);//ajax从‘隐藏字段’获取

                //查出来该id对应的用户名
                AdminUsers user = new AdminUsersBll().GetModel(id);

                //查出来所有“角色”
                RolesBll roleBll = new RolesBll();
                List<Roles> roles = roleBll.GetModelList("");

                //查出来，用户选中的"角色"
                AdminUserRolesBll userRolesBll = new AdminUserRolesBll();
                List<AdminUserRoles> selectedUserRoles = userRolesBll.GetModelList("AdminUserId=" + id);
                //绑定到checkbox 标签中，使用匿名类
                //用户选中“角色”的id的集合
                List<long> selectedRoles = new List<long>();
                foreach (AdminUserRoles userRole in selectedUserRoles)
                {
                    selectedRoles.Add(userRole.RoleId);
                }
                AdminHelper.CheckPower("编辑管理用户");
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/AdminUser/AdminUserRoles.cshtml",
                     new
                     {
                         action = "edit",
                         roles = roles,//加载出来所有的角色
                         id = id,
                         userName = user.UserName,
                         selectedRoles = selectedRoles,//添加用户
                         lable = "重新分配该用户的角色："
                     });
            }
            else if (action == "save")
            {
                string saveAction = context.Request["saveAction"];
                if (saveAction == "addnew")
                {
                    AdminHelper.CheckPower("新增管理用户");//避免恶意用户直接想addnew发送请求
                    //用户的新增添加在此处----------------：
                    #region 用户名、密码 新增
                    //1.1.1 服务器进行，用户名和密码验证，不能少的

                    //获得“用户名”
                    string username = context.Request["username"];
                    //获得“密码”
                    string password = context.Request["password"];

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))//1.1.3 
                    {
                        RPAjaxhelperCommons.WriteJson(context.Response, "error", "用户名和密码为必填！");
                        return;
                    }
                    if (username.Length < 2 || username.Length > 10)//1.1.2 应为给服务器端返回的数据比较多，这里封装一个工具类RPCommons
                    {
                        RPAjaxhelperCommons.WriteJson(context.Response, "error", "用户名长度必须介于2到10之间！");
                        return;
                    }
                    if (password.Length < 3 || password.Length > 20)//1.1.4 
                    {
                        RPAjaxhelperCommons.WriteJson(context.Response, "error", "密码长度必须介于2到20之间！");
                        return;
                    }

                    //1.1.5 通过了用户名的验证，下面开始执行，插入数据库的操作

                    //bll.GetModelList("UserName='"+username+"'");//打死都不能用，有漏洞
                    //1.1.7 自己写一个部分类，自己定义参数化查询，再执行

                    //1.1.8 定义部分类，完成之后
                    if (bll.IsUserNameExits(username))
                    {
                        RPAjaxhelperCommons.WriteJson(context.Response, "error", "用户名已经存在！");
                        return;
                    }
                    //1.1.9 这里进行，用户信息的插入操作
                    bll.AddUser(username, password);


                    #endregion

                    #region 用户添加的同时，对应的角色也随之添加上
                    //获得选中的角色的集合
                    string selectedRoles = context.Request["selectedRoles"];
                    //验证“角色”不能为空
                    if (string.IsNullOrWhiteSpace(selectedRoles))
                    {
                        RPAjaxhelperCommons.WriteJson(context.Response, "error", "至少选中一个角色名称！");
                        return;
                    }

                    #region 获得新增“用户”的id
                    AdminUsers user = bll.GetByUserName(username);
                    long userId = user.Id;

                    //拿到存入用户的id值
                    /*
                    //long userId = userBll.Add(user);//新增一个“用户”。add方法返回值是新增用户的id
                    */
                    //调试发现：这里出现了重复，添加用户的情况，，，真是“不会灵活应用啊！！”


                    #endregion


                    #region 新增  用户--角色  对应关系到表 T_AdminUserRoles
                    string[] strs = selectedRoles.Split(',');
                    long[] roleIds = new long[strs.Length];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        roleIds[i] = Convert.ToInt64(strs[i]);
                    }
                    AdminUserRolesBll userRolesBll = new AdminUserRolesBll();
                    userRolesBll.AddUserRoles(userId, roleIds); //用户Id，角色Id
                    #endregion
                    #endregion

                    #region 记录新增用户,以及用户的角色名称 到日志表中
                    //遍历该用户拥有的的角色名称
                    string rolesName = new AdminHelper().UserHavedRolesName(selectedRoles);

                    AdminHelper.RecordOperationLog("新增用户:“" + username + "”,该用户具有的角色:" + rolesName);
                    #endregion

                    RPAjaxhelperCommons.WriteJson(context.Response, "ok", "用户名，密码，以及用户拥有的角色，添加成功！");
                }
                else if (saveAction == "edit")
                {
                    AdminHelper.CheckPower("编辑管理用户");//避免恶意用户直接想edit发送请求

                    #region jQuery批量获用户取选中的“角色”,使用ajax传递到服务器端
                    string selectedRoles = context.Request["selectedRoles"];
                    #endregion

                    #region 点击“修改”按钮，此时将“修改用户”的id传递到服务器端
                    long userId = Convert.ToInt64(context.Request["id"]);
                    #endregion

                    #region 把传递过来的“角色”字符串，进行split，转化为字符串数组，for循环遍历，到一个声明的long[]数组中（因为ajax只是支持“字符串”传送）
                    //编辑用户、角色对应关系
                    string[] strs = selectedRoles.Split(',');
                    long[] userRoles = new long[strs.Length];
                    for (int i = 0; i < strs.Length; i++)
                    {
                        userRoles[i] = Convert.ToInt64(strs[i]);
                    }
                    #endregion

                    #region 根据"修改用户的id",先把“这个用户--角色关系”从表T_AdminUserRoles中都删掉
                    AdminUserRolesBll userRolesBll = new AdminUserRolesBll();
                    userRolesBll.ClearUsersId(userId);
                    #endregion

                    #region 使用“修改用户的id”，和用户已经“重新选择”的“角色集合long[]”,循环使用动软提供的Add方法添加到表T_AdminUserRoles中
                    //再重新添加角色。这样最简单（粗暴！）
                    userRolesBll.AddUserRoles(userId, userRoles); //用户Id，角色Id
                    #endregion

                    RPAjaxhelperCommons.WriteJson(context.Response, "ok", "用户拥有的角色，重新分配成功！");

                    #region 记录修改用户到日志表中
                    //遍历该用户拥有的的角色名称
                    string roleNameCollection = new AdminHelper().UserHavedRolesName(selectedRoles);
                    //查出来当前用户的名字
                    string userName = bll.GetModel(userId).UserName;
                    AdminHelper.RecordOperationLog("修改用户:“" + userName + "”,重新选择了的角色:" + roleNameCollection);
                    #endregion
                }
                else
                {
                    throw new Exception("saveAction错误！");
                }
            }
            else if ("delete" == action)
            {
                AdminHelper.CheckPower("删除管理用户");

                //a.1 获取地址栏递过来的id值
                string id = context.Request["id"];
                //a.2 防止漏洞，进行转换
                long idd = Convert.ToInt64(id);
                /*升级：：
                //a.3 调用bll层中的delete方法，进行删除
                bll.Delete(idd);
                 */
                //首先，删除表T_AdminUserRoles表中的用户的id（引用的表中的用户id）
                AdminUserRolesBll userBll = new AdminUserRolesBll();
                userBll.ClearUsersId(idd);//(被引用的表中的用户id)

                #region 记录删除用户到日志表中(删除程序执行前纪录)

                //查出来要删除用户的名字
                string userName = bll.GetModel(idd).UserName;
                AdminHelper.RecordOperationLog("删除用户:“" + userName + "”");
                #endregion

                //其次，删除表T_AdminUsers表中的用户id
                bll.Delete(idd);



                //a.4 删除成功后，跳转页面到用户list
                context.Response.Redirect("AdminUserController.ashx?action=list");
            }
            else if ("batchDelete" == action)
            {
                //检查权限，使用批量检查方法,返回的是Json
                AdminHelper.CheckPowerOnBatchButton(context, "批量删除管理用户");

                //2.0 获取Ajax传递过来的ids
                string ids = context.Request["ids"];// 1 2 3
                //2.1 需要做数组转化处理，目的：防止Sql漏洞攻击
                string[] strs = ids.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //返回这样的数组{ "1","2","3"}

                //2.3 字符串的数组的长度
                long[] idArray = new long[strs.Length];//bigInt
                for (int i = 0; i < idArray.Length; i++)
                {
                    idArray[i] = Convert.ToInt64(strs[i]);//{3,5,8}
                    //2.4 这是关键步骤，进行了防止漏洞的注入处理
                }

                //2.5 把数组转化为字符串指定间隔符的字符串
                string userIdList = string.Join(",", idArray);//"3,5,8"
                //delete from T_AdminUsers where id in (3,5,8);
                /*  升级此处：
                 * 2.6 调用bll层方法
                bll.DeleteList(idList);
                */

                #region 记录批量删除用户到日志表中(删除程序执行前纪录)
                //查询出来要批量删除的用户的名字
                string userNameCollection = AdminHelper.UserNames(userIdList);
                AdminHelper.RecordOperationLog("批量删除用户:“" + userNameCollection + "”");
                #endregion


                //(先批量删除，引用的表中的用户id)
                AdminUserRolesBll userRoleBll = new AdminUserRolesBll();
                userRoleBll.batchDeleteUserIDList(userIdList);//动软没有提供，删除批量用户的Id，自己添加
                //（再批量删除被引用的表中的用户id）
                AdminUsersBll userBll = new AdminUsersBll();
                userBll.DeleteList(userIdList);


                //2.7 返回消息
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "");//删除成功，不发送消息了，因为客户端就没有处理

            }
            else if ("disabled" == action)
            {
                AdminHelper.CheckPower("禁用管理用户");
                //UI 层代码，越少越好
                long id = Convert.ToInt64(context.Request["id"]);

                /*
                AdminUsers user = bll.GetModel(id);
                user.IsEnabled = false;
                bll.Update(user);  //处理代码最好写在，Bll层中
                */

                bll.Disable(id); //1.1.1.1禁用

                #region 记录禁用用户到日志表中
                //查出来禁用了哪个用户的名字
                string userName = bll.GetModel(id).UserName;
                AdminHelper.RecordOperationLog("禁用用户:“" + userName + "”");
                #endregion
                context.Response.Redirect("AdminUserController.ashx?action=list");//1.1.1.1 重定向到列表列
            }
            else if ("batchdisabled" == action)
            {
                //检查权限，使用批量检查方法,返回的是Json
                AdminHelper.CheckPowerOnBatchButton(context, "批量禁用管理用户");
                //3.0 获取Ajax传递过来的ids
                string ids = context.Request["ids"];// 1 2 3
                //3.1 需要做数组转化处理，目的：防止Sql漏洞攻击
                string[] strs = ids.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); //返回这样的数组{ "1","2","3"}

                //3.3 字符串的数组的长度
                long[] idArray = new long[strs.Length];//bigInt
                for (int i = 0; i < idArray.Length; i++)
                {
                    idArray[i] = Convert.ToInt64(strs[i]);//{3,5,8}
                    //3.4 这是关键步骤，进行了防止漏洞的注入处理
                }

                AdminUsers model = new AdminUsers();
                model.IsEnabled = false;
                string idList = string.Join(",", idArray);
                if (bll.UpdateBatch(model, idList))
                {
                    #region 记录批量禁用用户到日志表中
                    //查询出来要批量禁用的用户的名字
                    string userNameCollection = AdminHelper.UserNames(idList);
                    AdminHelper.RecordOperationLog("批量禁用用户:“" + userNameCollection + "”");
                    #endregion

                    RPAjaxhelperCommons.WriteJson(context.Response, "ok", "");//批量禁用成功，不发送消息了，因为客户端就没有处理  
                }
                else
                {
                    RPAjaxhelperCommons.WriteJson(context.Response, "error", "批量禁用失败！");
                }


                #region 考虑到用一条sql语句就可以到达，批量禁用的效果，这里进行了，修改，这是第一次写的代码！！！

                /*
                //3.5 遍历idArray数组，根据id调用Bll层中的GetModel方法，从数据库中查询出来，得到model
                for (long  i = 0; i < idArray.Length; i++)
                {
                    bll.Disable(idArray[i]);
                }
              
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "");//批量禁用成功，不发送消息了，因为客户端就没有处理
                */

                #endregion


            }
            else if (action == "editPassword")//当前用户密码修改
            {

                //获得新密码
                string newPass = context.Request["newPass"];
                //取得当前用户的model
                //先取得当前的用户名
                string userNamePresent = context.Request["userNamePresent"];
                AdminUsers userModel = bll.GetByUserName(userNamePresent);
                //将新密码存入到model中
                userModel.PassWord = RPCommonHelper.CalcMD5(newPass);
                //使用三层的update方法
                bll.Update(userModel);

                #region 记录当前用户修改自己的密码到日志表中
                AdminHelper.RecordOperationLog("修改了自己的密码!新密码为:" + newPass + "");
                #endregion

                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "" + newPass + "");
            }
            else if (action == "batchPassword")//批量重置密码
            {
                AdminHelper.CheckPowerOnBatchButton(context, "批量重置管理用户密码");
                #region 获得要批量更新密码的用户集合
                //3.0 获取Ajax传递过来的selectedUserIds
                string ids = context.Request["selectedUserIds"];// 1 2 3
                //3.1 需要做数组转化处理，目的：防止Sql漏洞攻击
                string[] strs = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); //返回这样的数组{ "1","2","3"}

                //3.3 字符串的数组的长度
                long[] idArray = new long[strs.Length];//bigInt
                for (int i = 0; i < idArray.Length; i++)
                {
                    idArray[i] = Convert.ToInt64(strs[i]);//{3,5,8}
                    //3.4 这是关键步骤，进行了防止漏洞的注入处理
                }
                //3.5 把数组转化为字符串指定间隔符的字符串
                string userList = string.Join(",", idArray);//"3,5,8"
                #endregion

                // 将选中的用户值，放入到一个session中
                context.Session["userList"] = userList;
                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "用户选中的用户集合已经存入到session中");
            }
            else if (action == "batchEditPassword")
            {
                //获得要批量更新的用户设置的“新的密码”
                string newPassword = context.Request["newPassword"];
                //验证
                if (string.IsNullOrWhiteSpace(newPassword))//1.1.4 
                {
                    RPAjaxhelperCommons.WriteJson(context.Response, "error", "密码不能为空！");
                    return;
                }
                if (newPassword.Length < 3 || newPassword.Length > 20)//1.1.4 
                {
                    RPAjaxhelperCommons.WriteJson(context.Response, "error", "密码长度必须介于2到20之间！");
                    return;
                }

                //获得批量更新的用户的id集合
                string adminUserList = context.Request["userList"];

                //调用批量更新方法
                bll.UpDateBactchPass(newPassword, adminUserList);

                #region 记录批量修改哪些用户的密码到日志表中
                //查询出来批量重置密码的用户的名字
                string userNameCollection = AdminHelper.UserNames(adminUserList);
                AdminHelper.RecordOperationLog("批量修改用户:“" + userNameCollection + "”的密码为：" + newPassword + "");
                #endregion

                RPAjaxhelperCommons.WriteJson(context.Response, "ok", "" + newPassword + "");


            }
            else
            { throw new Exception("action错误！"); }


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