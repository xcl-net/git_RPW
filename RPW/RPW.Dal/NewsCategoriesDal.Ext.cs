using Maticsoft.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Dal
{
    public partial class NewsCategoriesDal
    {

        public bool HasChildCategory(long catId)
        {
            DataSet d = DbHelperSQL.Query("select Id,Name,ParentId  from t_newsCategories where ParentId=@ParentId",
                  new SqlParameter("@ParentId", catId));
            if (d.Tables[0].Rows.Count > 0)//说明该类，有子类
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public RPW.Model.NewsCategories GetModelByParentId(long ParentId)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 Id,Name,ParentId from T_NewsCategories ");
            strSql.Append(" where ParentId=@ParentId");
            SqlParameter[] parameters = {
					new SqlParameter("@ParentId", SqlDbType.BigInt)
			};
            parameters[0].Value = ParentId;

            RPW.Model.NewsCategories model = new RPW.Model.NewsCategories();
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }
    }
}
