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
	public partial class FrontUserClassRoomsDal
	{


		/// <summary>
		/// 得到一个对象实体,通过学生的id
		/// </summary>
		public RPW.Model.FrontUserClassRooms GetModelByStuId(long classId)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("select  top 1 Id,ClasssRoomId,FrontUserId from T_FrontUserClassRooms ");
			strSql.Append(" where FrontUserId=@FrontUserId");
			SqlParameter[] parameters = {
					new SqlParameter("@FrontUserId", SqlDbType.BigInt,4)
			};
			parameters[0].Value = classId;

			RPW.Model.FrontUserClassRooms model = new RPW.Model.FrontUserClassRooms();
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
