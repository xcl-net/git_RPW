using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RPW.Bll;
using RPW.Model;

namespace RPW.Admin.ClassesMgr
{
    /// <summary>
    /// Classes 的摘要说明
    /// </summary>
    public class ClassesConroller : BaseHandler
    {
        //展示班级列表
        public void ListClasses(HttpContext context)
        {
            List<ClassRooms> classRooms = new ClassRoomsBll().GetModelList("");
            RPWRazor.RPRazorHelper.OutputRazor(context, "/ClassesMgr/ClassesList.cshtml", classRooms);
        }
        //新增班级
        public void AddClass(HttpContext context)
        {
            RPWRazor.RPRazorHelper.OutputRazor(context, "/ClassesMgr/ClassEditAddNew.cshtml",
                new { id = "", classRoomName = "", action = "AddClass" });
        }
        //编辑班级
        public void EditClass(HttpContext context)
        {
            string id = context.Request["id"];
            //根据班级id查出班级的名称
            ClassRooms classModel = new ClassRoomsBll().GetModel(Convert.ToInt32(id));
            //班级名称
            string classRoomName = classModel.ClassRoom;
            RPWRazor.RPRazorHelper.OutputRazor(context, "/ClassesMgr/ClassEditAddNew.cshtml",
               new { id = id, classRoomName = classRoomName, action = "EditClass" });
        }
        //保存班级
        public void SaveClass(HttpContext context)
        {
            string saveAction = context.Request["saveAction"];
            string classRoomName = context.Request["classRoomName"];
            string id = context.Request["id"];

            if (string.IsNullOrWhiteSpace(classRoomName))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "班级名称不能为空");
                return;
            }

            ClassRoomsBll classBll = new ClassRoomsBll();
            if (saveAction == "AddClass")
            {
                //插入
                ClassRooms classModel = new ClassRooms();
                classModel.ClassRoom = classRoomName;
                classBll.Add(classModel);
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "新增成功！");

            }
            else if (saveAction == "EditClass")
            {
                //根据id查出来，要编辑那个对象
                ClassRooms classModel = new ClassRoomsBll().GetModel(Convert.ToInt32(id));
                classModel.ClassRoom = classRoomName;
                //更新
                classBll.Update(classModel);
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "修改成功！");

            }
            else
            {
                throw new Exception("saveAction未知！");
            }

        }
        //删除班级
        public void DeleteClass(HttpContext context)
        {
            //获取id
            string id = context.Request["id"];
            ClassRoomsBll classBll = new ClassRoomsBll();
            classBll.Delete(Convert.ToInt32(id));
            context.Response.Redirect("/ClassesMgr/ClassesConroller.ashx?action=ListClasses");

        }

        //查看班级人员管理列表
        public void ListClassMembetMgr(HttpContext context)
        {
            List<ClassRooms> classMemberModels = new ClassRoomsBll().GetModelList("");
            RPWRazor.RPRazorHelper.OutputRazor(context, "/ClassesMgr/ClassesFrontusers.cshtml", classMemberModels);
        }

        //班级成员管理
        public void MemberMgr(HttpContext context)
        {
            //获取班级的id
            string id = context.Request["id"];
            //查出班级的名字
            ClassRooms classModel = new ClassRoomsBll().GetModel(Convert.ToInt32(id));
            string className = classModel.ClassRoom;
            RPWRazor.RPRazorHelper.OutputRazor(context, "/ClassesMgr/memberMgr.cshtml",
                new
                {
                    currentClassName = className,//显示标题
                    ClassId = id //加载班级的id到隐藏字段，供ajax请求使用
                });
        }

        //加载班级成员
        public void LoadMembers(HttpContext context)
        {
            //获取班级的id
            string id = context.Request["classId"];
            //页面打开，用ajax 请求，加载班级全部成员，当前班级的人员--查 班级--学生表
            List<FrontUserClassRooms> clasStuModels = new FrontUserClassRoomsBll().GetModelList("ClasssRoomId=" + id);
            //加载出来班级成员的id集合
            //返回的要求是班级成员名字的集合
            //因此，进行遍历

            //声明一个学生集合，接受，带名字的班级成员集合
            List<FrontUsers> list = new List<FrontUsers>();
            foreach (var clasStu in clasStuModels)
            {
                //遍历出来每个学生的id
                long studentId = clasStu.FrontUserId;
                //查找出来用户model
                FrontUsers frontUser = new FrontUsersBll().GetModel(studentId);
                //添加到list集合中
                list.Add(frontUser);
            }
            //返回集合到json
            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "", list);
        }

        //加载按条件搜索的结果列表
        public void ListResult(HttpContext context)
        {
            //获取搜索条件
            string txt = context.Request["txt"];


            if (string.IsNullOrWhiteSpace(txt))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "请输入搜索条件");
                return;
            }
            FrontUsersBll bll = new FrontUsersBll();
            List<FrontUsers> listByName = bll.GetModelListByNameFront(txt);
            if (listByName != null)
            {
                //返回集合到json
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "", listByName);
            }
            else
            {
                List<FrontUsers> bllByTeleNum = bll.GetModelListByTeleNum(txt);
                if (bllByTeleNum != null)
                {
                    //返回集合到json
                    RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "", bllByTeleNum);
                }
                else
                {
                    List<FrontUsers> bllByQQNum = bll.GetModelListByQQNum(txt);
                    if (bllByQQNum != null)
                    {
                        //返回集合到json
                        RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "", bllByQQNum);
                    }
                    else
                    {
                        //返回集合到json
                        RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "没有这个用户");
                    }
                }
            }
        }

        //点击后，添加到  班级成员列表  中
        public void AddToClass(HttpContext context)
        {
            //获取班级的id号
            string classId = context.Request["classId"];
            //获取ajax传递来的 "搜索结果" 字符串
            string str = context.Request["name"];
            string[] sb = str.Split('(');

            //截取到 用户名
            string sbName = sb[0];

            //通过 用户名 查出来用户的信息
            FrontUsers stuModel = new FrontUsersBll().GetModelByUserName(sbName);

            //添加到  班级--成员 表中
            FrontUserClassRoomsBll relation = new FrontUserClassRoomsBll();
            FrontUserClassRooms relationModel = new FrontUserClassRooms();
            //添加班级id关系
            relationModel.ClasssRoomId = Convert.ToInt32(classId);
            //添加学生id关系
            relationModel.FrontUserId = stuModel.Id;

            relation.Add(relationModel);

            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "添加关系成功");



        }

        //单击事件ajax请求 删除数据 从班级列表 中
        public void DeleteFromClass(HttpContext context)
        {
            string name = context.Request["name"];
            //通过 用户名 查出来用户的Model
            FrontUsers stuModel = new FrontUsersBll().GetModelByUserName(name);
            //学生的id
            long stuId = stuModel.Id;
            //根据学生的id，查出来  班级学生表的对应id的model
            FrontUserClassRoomsBll relation = new FrontUserClassRoomsBll();
            FrontUserClassRooms stuClaModel = relation.GetModelByStuId(stuId);
            //获取要删除的那个id列
            int id = stuClaModel.Id;
            //
            relation.Delete(id);
            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "");


        }
    }
}