using RPW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class AdminUsersBll
    {
        /// <summary>
        /// 根据用户名，获取用户的信息，返回到Model中，通过Model显示，到UI层中
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public AdminUsers GetByUserName(string username)//在dal.ext.cs文件中拷贝出这个方法
        {
            return dal.GetByUserName(username);
        }

        public bool IsUserNameExits(string username)
        {
            //if (GetByUserName(username) != null)  
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

            //简化写成一句
            return GetByUserName(username) != null;//这里是一个逻辑表达式  不空 ！= null

        }

        /// <summary>
        /// 新增一个用户，经过md5加密的
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void AddUser(string username, string password)
        {
            //md5处理放到Bll中，UI层中代码越少越好，组号只有获取用户输入。检测合法性，输出
            AdminUsers user = new AdminUsers();
            user.UserName = username;
            user.PassWord = RPWCommonts.RPCommonHelper.CalcMD5(password);
            user.IsEnabled = true;//默认设置为账户启用

            Add(user); //调用 动软自己生成“增加一条数据”的代码

        }
        /// <summary>
        /// 禁用一个账户
        /// </summary>
        /// <param name="id"></param>
        public void Disable(long id)
        {
            AdminUsers user = GetModel(id);//根据id从数据库中查询出来，放到model中，得到一个实体
            user.IsEnabled = false;    //false表示不启用该用户  //该变model中的IsEnable的值，
            Update(user);//动软生成的  然后，用Dal层中的Update方法，回到数据库中去修改对应的IsEnable值
        }

        //利用枚举返回，登录的结果
        public LoginResult Login(string username, string password)
        {
            AdminUsers user = GetByUserName(username);
            if (user == null)
            {
                return LoginResult.UserNameNotFound;
            }
            else if (user.PassWord != RPWCommonts.RPCommonHelper.CalcMD5(password))
            {
                return LoginResult.PasswordError;
            }
            else if (user.IsEnabled==false)
            {
                return LoginResult.UserNameDisabled;//用户名禁止
            }
            else
            {
                return LoginResult.OK;
            }
        }

        /// <summary>
        /// 批量更新多条数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idList">where条件 3,4,5 等等</param>
        /// <returns></returns>
        public bool UpdateBatch(RPW.Model.AdminUsers model, string idList)
        {
            return dal.UpdateBatch(model, idList);
        }
        /// <summary>
        /// 批量重置密码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        public bool UpdateBatchPassWord(RPW.Model.AdminUsers model, string idList)
        {
            return dal.UpdateBatchPassWord(model, idList);
        }
        /// <summary>
        /// 批量重置密码
        /// </summary>
        /// <param name="passWord">批量更改的新密码</param>
        /// <param name="idList">批量的id</param>
        public void UpDateBactchPass(string passWord, string idList)
        {
            //为了使用方法UpdateBatchPassWord（），首先，将密码存到model中
            AdminUsers user = new AdminUsers();
            user.PassWord = RPWCommonts.RPCommonHelper.CalcMD5(passWord);
            UpdateBatchPassWord(user, idList);
        }
        /// <summary>
        /// 判断当前用户的id，是否又指定的名称“权限”
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="powerName"></param>
        /// <returns></returns>
         public bool HasPower(long userId, string powerName)
        {

            return dal.HasPower(userId, powerName);
        }
    }

    //1.1 声明一个枚举，登录后出现的情况
    public enum LoginResult { OK, UserNameNotFound, PasswordError,UserNameDisabled };//添加禁止用户登录的情况
}
