using Maticsoft.DBUtility;
using RPW.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Dal
{
    public partial class AdminUsersDal
    {

        /// <summary>
        /// 根据用户名，获取用户的信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public AdminUsers GetByUserName(string username)
        {
            //1.1 从数据库中，根据名字查出来这个人的记录
            string sql = "select * from T_AdminUsers where UserName=@UserName";
            DataSet ds = DbHelperSQL.Query(sql, new SqlParameter("@UserName", username));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null; //没有查询出来就是null
            }
        }

        /// <summary>
        /// 批量更新多条数据的禁止选项
        /// </summary>
        public bool UpdateBatch(RPW.Model.AdminUsers model, string idList)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update T_AdminUsers set ");
            //strSql.Append("UserName=@UserName,");
            //strSql.Append("PassWord=@PassWord,");
            strSql.Append("IsEnabled=@IsEnabled");
            strSql.Append(" where Id in (").Append(idList).Append(")");
            SqlParameter[] parameters = {
					//new SqlParameter("@UserName", SqlDbType.NVarChar,50),
					//new SqlParameter("@PassWord", SqlDbType.NVarChar,50),
					new SqlParameter("@IsEnabled", SqlDbType.Bit,1)
                                        };

            //new SqlParameter("@idList", SqlDbType.BigInt,8)};
            // parameters[1].Value = model.PassWord;
            //parameters[0].Value = model.UserName;
            parameters[0].Value = model.IsEnabled;
            //parameters[3].Value = model.Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
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
        /// 批量重置多条数据的密码
        /// </summary>
        /// <param name="model">Model实体</param>
        /// <param name="idList">Id的字符串集合</param>
        /// <returns></returns>
        public bool UpdateBatchPassWord(RPW.Model.AdminUsers model, string idList)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update T_AdminUsers set ");
            //strSql.Append("UserName=@UserName,");
            strSql.Append("PassWord=@PassWord");
            // strSql.Append("IsEnabled=@IsEnabled");
            strSql.Append(" where Id in (").Append(idList).Append(")");
            SqlParameter[] parameters = {
					//new SqlParameter("@UserName", SqlDbType.NVarChar,50),
					new SqlParameter("@PassWord", SqlDbType.NVarChar,50),
					//new SqlParameter("@IsEnabled", SqlDbType.Bit,1)
                                        };

            //new SqlParameter("@idList", SqlDbType.BigInt,8)};
            parameters[0].Value = model.PassWord;//数组的第一个就是PassWord
            //parameters[0].Value = model.UserName;
            // parameters[0].Value = model.IsEnabled;
            //parameters[3].Value = model.Id;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
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
        /// 根据当前的用户判断，该用户是否有该权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="powerName"></param>
        /// <returns></returns>
        public bool HasPower(long userId, string powerName)
        {
            object obj = DbHelperSQL.GetSingle(@"select count(*) from 
                    (
	                    select distinct adminuserid from T_AdminUserRoles where roleid in
                        (select roleid from T_RolePowes 
                         where PowerId=(select id from T_Powers where Name=@PowerName))
                    ) au 
                where au.AdminUserId=@AdminUserId", new SqlParameter("@AdminUserId", userId), new SqlParameter("@PowerName", powerName));
            int count = Convert.ToInt32(obj);
            //if (count == 1)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return count > 0;

        }
    }
}
