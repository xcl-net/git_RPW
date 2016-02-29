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
    public partial class UserCoursesDal
    {
        /// <summary>
        /// 查看当前用户的id，有哪些可以使用的课程(在有效期限的)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserCourses> GetUserCourses(long userId)
        {
            DataSet ds = DbHelperSQL.Query("select * from T_UserCourses where  UserId=@UserId and ExpireDateTime>GetDate()", new SqlParameter("@UserId", userId));
            DataTable dt = ds.Tables[0];

            //用户学习卡表的model,放到list集合中
            List<UserCourses> list = new List<UserCourses>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(this.DataRowToModel(row));
            }
            return list;
        }
    }
}
