using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPW.Model;
namespace RPW.Bll
{
    public partial class RolePowesBll
    {
        /// <summary>
        /// 把角色拥有的权限，循环加入到T_RolePowers表中
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="powerIds"></param>
        public void AddRolePowers(long roleId,IEnumerable<long> powerIds)
        {
            foreach (long  powerId in powerIds)
            {
                RolePowes rolePower = new RolePowes();
                //存入到Model中
                rolePower.RoleId = roleId;
                rolePower.PowerId = powerId;
                //使用动软的“新增”方法，
                new RolePowesBll().Add(rolePower);
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        public void ClearRole(long roleId)
        {
            dal.ClearRole(roleId);
        }

                /// <summary>
        /// 批量删除数据,角色Id的集体删除
        /// </summary>
        public bool batchDeleteRoleList(string RoleIdList)
        {
            return dal.batchDeleteRoleList(RoleIdList);
        }
    }
}
