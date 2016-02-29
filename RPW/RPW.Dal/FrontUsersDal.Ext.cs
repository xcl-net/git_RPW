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
    public partial class FrontUsersDal
    {
        /// <summary>
        /// 根据用户名，获取用户的信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public FrontUsers GetModelByUserName(string username)
        {
            //1.1 从数据库中，根据名字查出来这个人的记录
            string sql = "select * from T_FrontUsers where Name=@UserName";
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
        /// 根据邮箱地址，获取用户的信息
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public FrontUsers GetModelByEmail(string email)
        {
            //1.1 从数据库中，根据邮箱地址查出来这个人的记录
            string sql = "select * from T_FrontUsers where Email=@Email";
            DataSet ds = DbHelperSQL.Query(sql, new SqlParameter("@Email", email));
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
        /// 根据姓名 获得数据列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="teleNum"></param>
        /// <param name="qq"></param>
        /// <returns></returns>
        public List<FrontUsers> GetModelListByNameFront(string name)
        {


            string str = "select Id,Name,Pwd,Email,RegDateTime,IsActive,Telephone,QQ,SchoolName,Specialty,Graduate,EnroalYear,RealName FROM T_FrontUsers  where Name like @str_1 or Name like @str_2 ";

            DataSet dt = DbHelperSQL.Query(str, new SqlParameter("@str_1", name + "%"), new SqlParameter("@str_2", "%" + name + "%"));

            //声明一个集合接收
            List<FrontUsers> list = new List<FrontUsers>();
            if (dt.Tables[0].Rows.Count > 0)//如果查询出来数据
            {
                foreach (DataRow row in dt.Tables[0].Rows)
                {
                    FrontUsers userModel = DataRowToModel(row);
                    list.Add(userModel);
                }
                return list;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据手机号 获得数据列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="teleNum"></param>
        /// <param name="qq"></param>
        /// <returns></returns>
        public List<FrontUsers> GetModelListByTeleNum(string teleNum)
        {


            string str = "select Id,Name,Pwd,Email,RegDateTime,IsActive,Telephone,QQ,SchoolName,Specialty,Graduate,EnroalYear,RealName FROM T_FrontUsers where Telephone like @Telephone+'%'  ";


            DataSet dt = DbHelperSQL.Query(str, new SqlParameter("@Telephone", teleNum));

            //声明一个集合接收
            List<FrontUsers> list = new List<FrontUsers>();
            if (dt.Tables[0].Rows.Count > 0)//如果查询出来数据
            {
                foreach (DataRow row in dt.Tables[0].Rows)
                {
                    FrontUsers userModel = DataRowToModel(row);
                    list.Add(userModel);
                }
                return list;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据QQ号 获得数据列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="teleNum"></param>
        /// <param name="qq"></param>
        /// <returns></returns>
        public List<FrontUsers> GetModelListByQQNum(string qq)
        {


            string str = "select Id,Name,Pwd,Email,RegDateTime,IsActive,Telephone,QQ,SchoolName,Specialty,Graduate,EnroalYear,RealName FROM T_FrontUsers  where  QQ like @QQ+'%' ";

            DataSet dt = DbHelperSQL.Query(str,
                new SqlParameter("@QQ", qq));
            //声明一个集合接收
            List<FrontUsers> list = new List<FrontUsers>();
            if (dt.Tables[0].Rows.Count > 0)//如果查询出来数据
            {
                foreach (DataRow row in dt.Tables[0].Rows)
                {
                    FrontUsers userModel = DataRowToModel(row);
                    list.Add(userModel);
                }
                return list;
            }
            else
            {
                return null;
            }
        }
    }
}
