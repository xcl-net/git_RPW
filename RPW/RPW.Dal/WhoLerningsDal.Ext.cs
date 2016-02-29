using Maticsoft.DBUtility;
using RPW.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Dal
{
    public partial class WhoLerningsDal
    {
        /// <summary>
        /// 获得前几行数据,改造的动软的方法
        /// </summary>
        public List<WhoLernings> GetListShowTop(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" Id,Name,LearningTime,SegmentName,SchoolName ");
            strSql.Append(" FROM T_WhoLernings ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder + " desc");
            DataSet ds = DbHelperSQL.Query(strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                List<WhoLernings> whos = new List<WhoLernings>();
                WhoLernings who = new WhoLernings();
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    who = DataRowToModel(item);
                    whos.Add(who);
                }
                return whos;
            }
            else
            {
                return null;
            }
        }
    }
}
