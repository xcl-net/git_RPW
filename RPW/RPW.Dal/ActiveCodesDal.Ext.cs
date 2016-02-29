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
    public partial class ActiveCodesDal
    {
        /// <summary>
        /// 根据用户名，查激活码
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActiveCodes GetCodeByName(string name)
        {
            DataSet ds = DbHelperSQL.Query("select * from T_ActiveCodes where Name=@name", new SqlParameter("@name", name));
            DataTable table = ds.Tables[0];  //结果集的第0张表
            if (table.Rows.Count <= 0)    //表中没有，返回数据
            {
                return null;
            }
            else//表中a，返回数据
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }

        }

        //写个判断时间的函数
        public long TimeValue(string zhuceTime, string jihuoTime)
        {
            object d = DbHelperSQL.GetSingle("SELECT DATEDIFF(MI,'" + zhuceTime + "','" + jihuoTime + "') as TimeValue");
            long time = Convert.ToInt64(d);
            return time;
        }
    }
}
