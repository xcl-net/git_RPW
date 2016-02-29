using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPW.Model;
using Maticsoft.DBUtility;
using System.Data;
using System.Data.SqlClient;

namespace RPW.Dal
{
   public partial class PowersDal
    {

        /// <summary>
        /// 根据权限名称，获取信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Powers GetByPowerName(string powerName)
        {
            //1.1 从数据库中，根据名字查出来这个人的记录
            string sql = "select * from T_Powers where Name=@Name";
            DataSet ds = DbHelperSQL.Query(sql, new SqlParameter("@Name", powerName));
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null; //没有查询出来就是null
            }
        }
     
    }
}
