using RPW.Model;
using System.Collections.Generic;

namespace RPW.Bll
{
    public partial class AdminUserRolesBll
    {
        /// <summary>
        /// 把用户拥有的角色，循环加入到T_AdminUserRoles表中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        public void AddUserRoles(long userId, IEnumerable<long> roleIds)
        {
            foreach (long roleId in roleIds)
            {
                AdminUserRoles userRole = new AdminUserRoles();
                //存入到Model中
                userRole.AdminUserId = userId;
                userRole.RoleId = roleId;
                //使用动软的“新增”方法，
                new AdminUserRolesBll().Add(userRole);
            }
        }
        /// <summary>
        /// 删除用户的Id在表格 T_AdminUserRoles 中
        /// </summary>
        /// <param name="roleId"></param>
        public void ClearUsersId(long userId)
        {
            dal.ClearUserID(userId);
        }



        /// <summary>
        /// 批量删除数据,用户Id的集体删除
        /// </summary>
        public bool batchDeleteUserIDList(string userIdList)
        {
            return dal.batchDeleteUserIDList(userIdList);
        }


        /// <summary>
        /// 删除角色的Id在表格 T_AdminUserRoles 中
        /// </summary>
        /// <param name="userId"></param>
        public void ClearRolesId(long roleId)
        {
            dal.ClearRolesId(roleId);
        }


        /// <summary>
        /// 批量删除数据,角色Id的集体删除
        /// </summary>
        public bool batchDeleteRoleIDList(string roleIdList)
        {
            return dal.batchDeleteRoleIDList(roleIdList);
        }

    }
}
