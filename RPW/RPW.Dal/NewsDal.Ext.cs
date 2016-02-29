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
    public partial class NewsDal
    {


        /// <summary>
        /// 删除一条数据(根据catid)
        /// </summary>
        public bool DeleteByCategoryId(long Id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from T_News ");
            strSql.Append(" where CategoryId=@CategoryId");
            SqlParameter[] parameters = {
                    new SqlParameter("@CategoryId", SqlDbType.BigInt)
            };
            parameters[0].Value = Id;

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
        ///  获取catId=catId的数据,根据id排序
        /// </summary>
        /// <param name="catId">新闻属于哪个类</param>
        /// <param name="startRowNum">本页第一条记录</param>
        /// <param name="finishRowNum">本页最后一条记录</param>
        /// <returns></returns>
        public List<News> GetPageNewsList(long catId, long startRowNum, long finishRowNum)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"select * from(
                                         select ROW_NUMBER() over(order by id) rownum,*  
                                         from T_News 
                                         where CategoryId=@catId
                                         ) t
                        where t.rownum>=@startRowNum and t.rownum<=@finishRowNum ");
            DataSet ds = DbHelperSQL.Query(sb.ToString(),
                new SqlParameter("@startRowNum", startRowNum),
                new SqlParameter("@finishRowNum", finishRowNum),
                new SqlParameter("@catId", catId));

            //DataSet 转换到 Model模型中去，这样符合三层设计思想
            List<News> list = new List<News>();
            foreach (DataRow datarow in ds.Tables[0].Rows)
            {
                list.Add(this.DataRowToModel(datarow));
            }

            return list;

        }
    }
}
