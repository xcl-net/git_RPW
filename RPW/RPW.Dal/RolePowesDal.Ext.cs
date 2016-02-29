using Maticsoft.DBUtility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Dal
{
   public partial class RolePowesDal
    {
        /// <summary>
        /// 删除角色Dal层
        /// </summary>
        /// <param name="roleId"></param>
        public void ClearRole(long roleId)
        {
            DbHelperSQL.ExecuteSql("delete from T_RolePowes where RoleId=@RoleId ", new SqlParameter("@RoleId", roleId));
        }

        /// <summary>
        /// 批量删除数据,角色Id的集体删除
        /// </summary>
        public bool batchDeleteRoleList(string RoleIdList)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_RolePowes ");
            strSql.Append(" where RoleId in (" + RoleIdList + ")  ");
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
