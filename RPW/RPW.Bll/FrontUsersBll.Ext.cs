using RPW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class FrontUsersBll
    {


        /// <summary>
        /// 根据用户名，获取用户的信息，返回到Model中，通过Model显示，到UI层中
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public FrontUsers GetModelByUserName(string username)//在dal.ext.cs文件中拷贝出这个方法
        {
            return dal.GetModelByUserName(username);
        }

        /// <summary>
        /// 注册时候，根据用户名判断用户名是否存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CheckUserNameOnReg(string username)
        {
            return GetModelByUserName(username) == null;
        }

        public long AddFrontUser(string name, string password, string email, string telephone)
        {

            FrontUsers user = new FrontUsers();
            user.Name = name;
            user.Pwd = RPWCommonts.RPCommonHelper.CalcMD5(password);
            user.Email = email;
            user.Telephone = telephone;
            user.RegDateTime = DateTime.Now;
            user.IsActive = false; //用户注册，默认未激活！

            //插入
            return Add(user);

        }
        /// <summary>
        /// 激活用户
        /// </summary>
        /// <param name="name"></param>
        public void Active(string name)
        {
            FrontUsers user = GetModelByUserName(name);
            //做检查，防止数据库中没有这个用户的信息；
            if (user == null)
            {
                throw new Exception("没有" + name + "的信息！");
            }
            user.IsActive = true;
            //更新
            new FrontUsersBll().Update(user);


        }


        //利用枚举返回，登录的结果
        public FrontLoginResult Login(string username, string password)
        {
            FrontUsers user = GetModelByUserName(username);
            if (user == null)
            {
                return FrontLoginResult.UserNameNotFound;
            }
            else if (user.Pwd != RPWCommonts.RPCommonHelper.CalcMD5(password))
            {
                return FrontLoginResult.PasswordError;
            }
            else if (user.IsActive == false)
            {
                return FrontLoginResult.UserNameDisActive;//用户名未激活
            }
            else
            {
                return FrontLoginResult.OK;
            }
        }
        /// <summary>
        /// 根据邮箱地址，取得用户的记录
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public FrontUsers GetModelByEmail(string email)
        {
            return dal.GetModelByEmail(email);
        }


        /// <summary>
        /// 根据姓名 获得数据列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="teleNum"></param>
        /// <param name="qq"></param>
        /// <returns></returns>
        public List<FrontUsers> GetModelListByNameFront(string name)
        {
            return dal.GetModelListByNameFront(name);
        }

        /// <summary>
        /// 根据手机号 获得数据列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="teleNum"></param>
        /// <param name="qq"></param>
        /// <returns></returns>
        public List<FrontUsers> GetModelListByTeleNum(string teleNum)
        {
            return dal.GetModelListByTeleNum(teleNum);
        }

        /// <summary>
        /// 根据QQ号 获得数据列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="teleNum"></param>
        /// <param name="qq"></param>
        /// <returns></returns>
        public List<FrontUsers> GetModelListByQQNum(string qq)
        {
            return dal.GetModelListByQQNum(qq);
        }
    }
    //声明一个枚举，登录后出现的情况
    public enum FrontLoginResult { OK, UserNameNotFound, PasswordError, UserNameDisActive };//添加未激活用户登录的情况
}
