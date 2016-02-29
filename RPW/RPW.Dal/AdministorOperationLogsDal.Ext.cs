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
    public partial class AdministorOperationLogsDal
    {
        #region 使用表格form来做的
        /// <summary>
        /// 时间段、用户名 搜索
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeFinish"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataSet SerchByNameTime(string timeStart, string timeFinish, long userId)//拼接sql语句，不需要参数传值
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("select a.Id,a.UserId,a.OperateDate,a.Description  from (select Id,OperateDate,UserId,Description  from T_AdministorOperationLogs where OperateDate between '");
            sb.Append(timeStart).Append(" 00:00:00'");
            sb.Append(" and  '").Append(timeFinish).Append(" 23:59:59' )").Append(" a ");
            sb.Append(" where a.UserId=").Append(@userId);
            return DbHelperSQL.Query(sb.ToString());

        }



        /// <summary>
        /// 时间段、关键词 搜索
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeFinish"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public DataSet SerchByKeywordTime(string timeStart, string timeFinish, string keyword)//拼接sql语句，不需要参数传值
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("select a.Id,a.UserId,a.OperateDate,a.Description  from (select Id,OperateDate,UserId,Description  from T_AdministorOperationLogs where OperateDate between '");
            sb.Append(timeStart).Append(" 00:00:00'");
            sb.Append(" and  '").Append(timeFinish).Append(" 23:59:59' )").Append(" a ");
            sb.Append(" where a.Description  like  '%").Append(keyword).Append("%'");
            return DbHelperSQL.Query(sb.ToString());

        }

        /*
 select b.Id,b.UserId,b.OperateDate,b.Description from (select a.Id,a.UserId,a.OperateDate,a.Description from (select Id,OperateDate,UserId,Description from T_AdministorOperationLogs 
where OperateDate between '2015-08-07 00:00:00'  and  '2015-08-08  23:59:59' ) a
where a.Description  like  '%新增%') b
where b.UserId=89        
         
         */
        /// <summary>
        /// 时间段、关键词、用户名Id
        /// </summary>
        /// <param name="timeStart">开始时间</param>
        /// <param name="timeFinish">结束时间</param>
        /// <param name="keyword">关键词</param>
        /// <param name="userId">用户Id</param
        /// <returns></returns>
        public DataSet SerchByKeyTimeUserName(string timeStart, string timeFinish, string keyword, long userId)//拼接sql语句，不需要参数传值
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select b.Id,b.UserId,b.OperateDate,b.Description from (select a.Id,a.UserId,a.OperateDate,a.Description from (select Id,OperateDate,UserId,Description from T_AdministorOperationLogs where OperateDate between '");
            sb.Append(timeStart).Append(" 00:00:00'");
            sb.Append(" and  '").Append(timeFinish).Append(" 23:59:59' )").Append(" a ");
            sb.Append(" where a.Description  like  '%").Append(keyword).Append("%' )").Append(" b ");
            sb.Append(" where b.UserId=").Append(userId);
            return DbHelperSQL.Query(sb.ToString());
        }

        #endregion

        #region 使用ajax请求来做的日志查询
        /// <summary>
        /// 使用ajax请求来做的日志查询
        /// </summary>
        /// <returns></returns>
        public List<AdminOpreResultLogs> GetModelListByAjax(string where, List<SqlParameter> parameters)
        {
            string sql = "select b.UserName Name, a.OperateDate OperaTime,a.Description Description from T_AdministorOperationLogs a left join T_AdminUsers b on a.UserId=b.Id ";
            if (!string.IsNullOrWhiteSpace(where))
            {
                sql = sql + "\r\n where 1=1 " + where;//
            }


            DataSet ds = DbHelperSQL.Query(sql, parameters.ToArray());

            DataTable tb = ds.Tables[0];

            //申明一个list集合接收
            List<AdminOpreResultLogs> list = new List<AdminOpreResultLogs>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                //这里的属性 Name，Description，OperaTime是自己定义的名字，model里没有定义，所以，这个需要在这里进行字段的声明，否则ui调用会出问题；
                AdminOpreResultLogs log = new AdminOpreResultLogs();
                log.Name = (string)row["Name"];//使用显示转换，却表Name的数据类型就是string，否则报错。Name为int就会错，用ToString(),就不用考虑这个问题；
                log.OperaTime = (DateTime)row["OperaTime"];
                log.Description = (string)row["Description"];
                list.Add(log);
            }
            return list;


        }

        /// <summary>
        /// 根据条件，显示日志信息
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public List<AdminOpreResultLogs> GetModelListByAjax_option(AdminOperationLogSearchOption option)
        {
            //完成接收 不同条件的接收
            //声明一个 变量用来接收 where条件
            string where = "";
            //声明一个参数集合接收，参数
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (option.SearchByUserName)//勾选了用户名
            {
                string username = option.UserName;
                where = where + "\r\n and b.UserName=@UserName";
                parameters.Add(new SqlParameter() { ParameterName = "@UserName", Value = username });//添加一个SqlParameter对象

            }
            if (option.SearchByCreateDateTime) //勾选了时间
            {
                //获取开始时间
                //结束时间

                where = where + "\r\n and (a.OperateDate>=@StartTime and a.OperateDate<=@EndTime)";
                parameters.Add(new SqlParameter() { ParameterName = "@StartTime", Value = option.StartTime });
                parameters.Add(new SqlParameter() { ParameterName = "@EndTime", Value = option.EndTime });

            }
            if (option.SearchByDesc)
            {
                string desc = option.Description;
                where = where + "\r\n and a.Description like @Desc";   //like '%@Desc%'→"%"+desc+"%" 正确的参数化替换方法
                parameters.Add(new SqlParameter() { ParameterName = "@Desc", Value = "%" + desc + "%" });
            }

            return GetModelListByAjax(where, parameters);
        }
        #endregion
    }
}
