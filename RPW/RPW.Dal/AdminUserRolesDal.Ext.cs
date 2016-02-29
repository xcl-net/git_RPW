using Maticsoft.DBUtility;
using System.Data.SqlClient;
using System.Text;

namespace RPW.Dal
{
    public partial class AdminUserRolesDal
    {
        /// <summary>
        /// 删除用户的Id在表格 T_AdminUserRoles 中
        /// </summary>
        /// <param name="userId"></param>
        public void ClearUserID(long userId)
        {
            DbHelperSQL.ExecuteSql("delete from T_AdminUserRoles where AdminUserId=@AdminUserId ", new SqlParameter("@AdminUserId", userId));
        }

        /// <summary>
        /// 删除角色的Id在表格 T_AdminUserRoles 中
        /// </summary>
        /// <param name="userId"></param>
        public void ClearRolesId(long roleId)
        {
            DbHelperSQL.ExecuteSql("delete from T_AdminUserRoles where RoleId=@RoleId ", new SqlParameter("@RoleId", roleId));
        }

        /// <summary>
        /// 批量删除数据,用户Id的集体删除
        /// </summary>
        public bool batchDeleteUserIDList(string userIdList)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_AdminUserRoles ");
            strSql.Append(" where AdminUserId in (" + userIdList + ")  ");
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 批量删除数据,角色Id的集体删除
        /// </summary>
        public bool batchDeleteRoleIDList(string roleIdList)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_AdminUserRoles ");
            strSql.Append(" where RoleId in (" + roleIdList + ")  ");
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
