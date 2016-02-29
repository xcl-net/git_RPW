/**  版本信息模板在安装目录下，可自行修改。
* FrontUsersDal.cs
*
* 功 能： N/A
* 类 名： FrontUsersDal
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/9/13 23:21:54   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Maticsoft.DBUtility;//Please add references
namespace RPW.Dal
{
	/// <summary>
	/// 数据访问类:FrontUsersDal
	/// </summary>
	public partial class FrontUsersDal
	{
		public FrontUsersDal()
		{}
		#region  BasicMethod

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(long Id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from T_FrontUsers");
			strSql.Append(" where Id=@Id");
			SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt)
			};
			parameters[0].Value = Id;

			return DbHelperSQL.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public long Add(RPW.Model.FrontUsers model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into T_FrontUsers(");
			strSql.Append("Name,Pwd,Email,RegDateTime,IsActive,Telephone,QQ,SchoolName,Specialty,Graduate,EnroalYear,RealName)");
			strSql.Append(" values (");
			strSql.Append("@Name,@Pwd,@Email,@RegDateTime,@IsActive,@Telephone,@QQ,@SchoolName,@Specialty,@Graduate,@EnroalYear,@RealName)");
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50),
					new SqlParameter("@Pwd", SqlDbType.NVarChar,50),
					new SqlParameter("@Email", SqlDbType.NVarChar,100),
					new SqlParameter("@RegDateTime", SqlDbType.DateTime),
					new SqlParameter("@IsActive", SqlDbType.Bit,1),
					new SqlParameter("@Telephone", SqlDbType.VarChar,13),
					new SqlParameter("@QQ", SqlDbType.VarChar,20),
					new SqlParameter("@SchoolName", SqlDbType.NVarChar,100),
					new SqlParameter("@Specialty", SqlDbType.NVarChar,100),
					new SqlParameter("@Graduate", SqlDbType.Int,4),
					new SqlParameter("@EnroalYear", SqlDbType.Int,4),
					new SqlParameter("@RealName", SqlDbType.NVarChar,50)};
			parameters[0].Value = model.Name;
			parameters[1].Value = model.Pwd;
			parameters[2].Value = model.Email;
			parameters[3].Value = model.RegDateTime;
			parameters[4].Value = model.IsActive;
			parameters[5].Value = model.Telephone;
			parameters[6].Value = model.QQ;
			parameters[7].Value = model.SchoolName;
			parameters[8].Value = model.Specialty;
			parameters[9].Value = model.Graduate;
			parameters[10].Value = model.EnroalYear;
			parameters[11].Value = model.RealName;

			object obj = DbHelperSQL.GetSingle(strSql.ToString(),parameters);
			if (obj == null)
			{
				return 0;
			}
			else
			{
				return Convert.ToInt64(obj);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(RPW.Model.FrontUsers model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update T_FrontUsers set ");
			strSql.Append("Name=@Name,");
			strSql.Append("Pwd=@Pwd,");
			strSql.Append("Email=@Email,");
			strSql.Append("RegDateTime=@RegDateTime,");
			strSql.Append("IsActive=@IsActive,");
			strSql.Append("Telephone=@Telephone,");
			strSql.Append("QQ=@QQ,");
			strSql.Append("SchoolName=@SchoolName,");
			strSql.Append("Specialty=@Specialty,");
			strSql.Append("Graduate=@Graduate,");
			strSql.Append("EnroalYear=@EnroalYear,");
			strSql.Append("RealName=@RealName");
			strSql.Append(" where Id=@Id");
			SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50),
					new SqlParameter("@Pwd", SqlDbType.NVarChar,50),
					new SqlParameter("@Email", SqlDbType.NVarChar,100),
					new SqlParameter("@RegDateTime", SqlDbType.DateTime),
					new SqlParameter("@IsActive", SqlDbType.Bit,1),
					new SqlParameter("@Telephone", SqlDbType.VarChar,13),
					new SqlParameter("@QQ", SqlDbType.VarChar,20),
					new SqlParameter("@SchoolName", SqlDbType.NVarChar,100),
					new SqlParameter("@Specialty", SqlDbType.NVarChar,100),
					new SqlParameter("@Graduate", SqlDbType.Int,4),
					new SqlParameter("@EnroalYear", SqlDbType.Int,4),
					new SqlParameter("@RealName", SqlDbType.NVarChar,50),
					new SqlParameter("@Id", SqlDbType.BigInt,8)};
			parameters[0].Value = model.Name;
			parameters[1].Value = model.Pwd;
			parameters[2].Value = model.Email;
			parameters[3].Value = model.RegDateTime;
			parameters[4].Value = model.IsActive;
			parameters[5].Value = model.Telephone;
			parameters[6].Value = model.QQ;
			parameters[7].Value = model.SchoolName;
			parameters[8].Value = model.Specialty;
			parameters[9].Value = model.Graduate;
			parameters[10].Value = model.EnroalYear;
			parameters[11].Value = model.RealName;
			parameters[12].Value = model.Id;

			int rows=DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
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
		/// 删除一条数据
		/// </summary>
		public bool Delete(long Id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from T_FrontUsers ");
			strSql.Append(" where Id=@Id");
			SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt)
			};
			parameters[0].Value = Id;

			int rows=DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
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
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string Idlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from T_FrontUsers ");
			strSql.Append(" where Id in ("+Idlist + ")  ");
			int rows=DbHelperSQL.ExecuteSql(strSql.ToString());
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
		/// 得到一个对象实体
		/// </summary>
		public RPW.Model.FrontUsers GetModel(long Id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select  top 1 Id,Name,Pwd,Email,RegDateTime,IsActive,Telephone,QQ,SchoolName,Specialty,Graduate,EnroalYear,RealName from T_FrontUsers ");
			strSql.Append(" where Id=@Id");
			SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt)
			};
			parameters[0].Value = Id;

			RPW.Model.FrontUsers model=new RPW.Model.FrontUsers();
			DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				return DataRowToModel(ds.Tables[0].Rows[0]);
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public RPW.Model.FrontUsers DataRowToModel(DataRow row)
		{
			RPW.Model.FrontUsers model=new RPW.Model.FrontUsers();
			if (row != null)
			{
				if(row["Id"]!=null && row["Id"].ToString()!="")
				{
					model.Id=long.Parse(row["Id"].ToString());
				}
				if(row["Name"]!=null)
				{
					model.Name=row["Name"].ToString();
				}
				if(row["Pwd"]!=null)
				{
					model.Pwd=row["Pwd"].ToString();
				}
				if(row["Email"]!=null)
				{
					model.Email=row["Email"].ToString();
				}
				if(row["RegDateTime"]!=null && row["RegDateTime"].ToString()!="")
				{
					model.RegDateTime=DateTime.Parse(row["RegDateTime"].ToString());
				}
				if(row["IsActive"]!=null && row["IsActive"].ToString()!="")
				{
					if((row["IsActive"].ToString()=="1")||(row["IsActive"].ToString().ToLower()=="true"))
					{
						model.IsActive=true;
					}
					else
					{
						model.IsActive=false;
					}
				}
				if(row["Telephone"]!=null)
				{
					model.Telephone=row["Telephone"].ToString();
				}
				if(row["QQ"]!=null)
				{
					model.QQ=row["QQ"].ToString();
				}
				if(row["SchoolName"]!=null)
				{
					model.SchoolName=row["SchoolName"].ToString();
				}
				if(row["Specialty"]!=null)
				{
					model.Specialty=row["Specialty"].ToString();
				}
				if(row["Graduate"]!=null && row["Graduate"].ToString()!="")
				{
					model.Graduate=int.Parse(row["Graduate"].ToString());
				}
				if(row["EnroalYear"]!=null && row["EnroalYear"].ToString()!="")
				{
					model.EnroalYear=int.Parse(row["EnroalYear"].ToString());
				}
				if(row["RealName"]!=null)
				{
					model.RealName=row["RealName"].ToString();
				}
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select Id,Name,Pwd,Email,RegDateTime,IsActive,Telephone,QQ,SchoolName,Specialty,Graduate,EnroalYear,RealName ");
			strSql.Append(" FROM T_FrontUsers ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperSQL.Query(strSql.ToString());
		}

		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public DataSet GetList(int Top,string strWhere,string filedOrder)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ");
			if(Top>0)
			{
				strSql.Append(" top "+Top.ToString());
			}
			strSql.Append(" Id,Name,Pwd,Email,RegDateTime,IsActive,Telephone,QQ,SchoolName,Specialty,Graduate,EnroalYear,RealName ");
			strSql.Append(" FROM T_FrontUsers ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			strSql.Append(" order by " + filedOrder);
			return DbHelperSQL.Query(strSql.ToString());
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) FROM T_FrontUsers ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			object obj = DbHelperSQL.GetSingle(strSql.ToString());
			if (obj == null)
			{
				return 0;
			}
			else
			{
				return Convert.ToInt32(obj);
			}
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.Id desc");
			}
			strSql.Append(")AS Row, T.*  from T_FrontUsers T ");
			if (!string.IsNullOrEmpty(strWhere.Trim()))
			{
				strSql.Append(" WHERE " + strWhere);
			}
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
			return DbHelperSQL.Query(strSql.ToString());
		}

		/*
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		{
			SqlParameter[] parameters = {
					new SqlParameter("@tblName", SqlDbType.VarChar, 255),
					new SqlParameter("@fldName", SqlDbType.VarChar, 255),
					new SqlParameter("@PageSize", SqlDbType.Int),
					new SqlParameter("@PageIndex", SqlDbType.Int),
					new SqlParameter("@IsReCount", SqlDbType.Bit),
					new SqlParameter("@OrderType", SqlDbType.Bit),
					new SqlParameter("@strWhere", SqlDbType.VarChar,1000),
					};
			parameters[0].Value = "T_FrontUsers";
			parameters[1].Value = "Id";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

		#endregion  BasicMethod
		#region  ExtensionMethod

		#endregion  ExtensionMethod
	}
}

