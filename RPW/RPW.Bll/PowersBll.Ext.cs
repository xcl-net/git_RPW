using RPW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
   public partial class PowersBll
    {

       Powers power = new Powers();
       /// <summary>
        /// 根据权限名称，获取信息
       /// </summary>
       /// <param name="powerName"></param>
       /// <returns></returns>
       public Powers GetByPowerName(string powerName)
       {
           return dal.GetByPowerName(powerName);
       }

       /// <summary>
       /// 判断权限名称是否已经存在，返回true false
       /// </summary>
       /// <param name="powerName"></param>
       /// <returns></returns>
       public bool IsPowerNameExits(string powerName)
       {

           return GetByPowerName(powerName) != null;//这里是一个逻辑表达式  不空 ！= null

       }
       /// <summary>
       /// 新增一个权限名称，因为动软的方法，要传递model，所以这个方法是为了将新的权限名称，先赋值到model中
       /// </summary>
       /// <param name="powerName">权限名称</param>
       public void AddPower(string powerName)
       {
           power.Name = powerName;

           //这里可以传递model了
           Add(power); //调用 动软自己生成“增加一条数据”的代码

       }

       /// <summary>
       /// 权限名称是否更新成功
       /// </summary>
       /// <param name="id"></param>
       /// <param name="powerName"></param>
       /// <returns></returns>
       public bool IsUpdae(long id,string powerName)
       { 
           //将从Ui层传递过来，要更新的Id,和新的权限名称存入到model
           power.Id=id;
           power.Name=powerName;
           //调用三层的更新方法
           return Update(power);
       }
    

    }
}
