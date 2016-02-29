using RPW.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace RPW.Bll
{
    public partial class AdministorOperationLogsBll
    {
        #region 判断时间的开始和结束，如果有一个时间是空的，就不给查询
        /// <summary>
        /// 判断时间的开始和结束，如果有一个时间是空的，就不给查询
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeFinish"></param>
        /// <returns></returns>
        public bool IsTwoNoNullTime(string timeStart, string timeFinish)
        {
            if (!string.IsNullOrWhiteSpace(timeStart) && !string.IsNullOrWhiteSpace(timeFinish))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 用户名 搜索日志信息
        /// <summary>
        /// 按用户名查询日志
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public List<AdministorOperationLogs> SearchByUsername(string username)
        {
            long Userid = GetUserNameId(username);
            return GetModelList(" UserId=" + Userid);
        }
        #endregion

        #region 关键词 搜索日志信息
        /// <summary>
        /// 根据关键词查询出来
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public List<AdministorOperationLogs> SearchByKeyWord(string keyTxt)
        {
            //查询出来关键词
            return GetModelList(" Description like '%" + keyTxt + "%'");
        }
        #endregion

        #region 用户名、关键字 搜索日志信息
        /// <summary>
        /// 用户名、关键字
        /// </summary>
        /// <param name="username"></param>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public List<AdministorOperationLogs> SerchByUserKEY(string username, string keyTxt)
        {

            long Userid = GetUserNameId(username);
            return GetModelList(" UserId=" + Userid + " and " + " Description like '%" + keyTxt + "%'");
        }
        #endregion

        #region 时间段 搜索日志信息
        /// <summary>
        /// 根据传入的开始时间、结束时间查询出来数据
        /// </summary>
        /// <param name="timeStrat"></param>
        /// <param name="timeFinish"></param>
        /// <returns></returns>
        public List<AdministorOperationLogs> SearchByTime(string timeWhere)
        {

            return GetModelList(timeWhere);
        }
        #endregion

        #region 时间段、用户名 搜索日志信息
        /// <summary>
        /// 时间段、用户名 搜索日志信息
        /// </summary>
        /// <param name="timeStart">起始时间</param>
        /// <param name="timeFinish">结束时间</param>
        /// <param name="userId">用户的id</param>
        /// <returns></returns>
        public List<RPW.Model.AdministorOperationLogs> SearchByNameTime(string timeStart, string timeFinish, long userId)
        {                                                                    //浪费了时间

            DataSet ds = dal.SerchByNameTime(timeStart, timeFinish, userId);
            return DataTableToList(ds.Tables[0]);
        }
        #endregion

        #region 时间段、关键词 搜索日志信息
        /// <summary>
        /// 时间段、关键词 搜索日志信息
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeFinish"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<RPW.Model.AdministorOperationLogs> SearchByKeywordTime(string timeStart, string timeFinish, string keyword)
        {                                                                         //轻松搞定

            DataSet ds = dal.SerchByKeywordTime(timeStart, timeFinish, keyword);
            return DataTableToList(ds.Tables[0]);
        }
        #endregion

        #region 时间段、关键词、用户名 搜索日志信息
        /// <summary>
        /// 时间段、关键词、用户名
        /// </summary>
        /// <param name="timeStart">开始时间</param>
        /// <param name="timeFinish">结束时间</param>
        /// <param name="keyword">关键词</param>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public List<RPW.Model.AdministorOperationLogs> SerchByKeyTimeUserName(string timeStart, string timeFinish, string keyword, string username)
        {
            //用户转化为用户的UserId
            long userId = GetUserNameId(username);
            DataSet ds = dal.SerchByKeyTimeUserName(timeStart, timeFinish, keyword, userId);
            return DataTableToList(ds.Tables[0]);

        }
        #endregion



        #region  根据 用户名 查出用户的Userid
        /// <summary>
        /// 根据 用户名 查出用户的Userid
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public long GetUserNameId(string username)
        {
            AdminUsersBll userBll = new AdminUsersBll();
            AdminUsers user = userBll.GetByUserName(username);
            if (user == null)//如果用户输入一个不存在的用户名后，根据用户名是取不到这个model对象的，那就需要对model非空验证，空的话，返回零，经过调试发现的问题。
            {
                return 0;
            }
            else
            {
                return userBll.GetByUserName(username).Id;
            }

        }
        #endregion

        #region 根据 选择的关键词Id获得关键词的具体的文本内容
        /// <summary>
        /// 根据 输入的关键词Id获得名字
        /// </summary>
        /// <param name="keyId"></param>
        /// <returns></returns>
        public string GetKeyword(long keyId)
        {
            KeyWords keyModel = new KeyWordsBll().GetModel(keyId);
            return keyModel.KeyWord;
        }
        #endregion

        #region 使用ajax请求来做的日志查询
        /// <summary>
        /// 使用ajax请求来做的日志查询
        /// </summary>
        /// <returns></returns>
        public List<AdminOpreResultLogs> GetModelListByAjax(string where, List<SqlParameter> parameters)
        {
            return dal.GetModelListByAjax(where, parameters);
        }
        /// <summary>
        /// 根据条件，显示日志信息
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public List<AdminOpreResultLogs> GetModelListByAjax_option(AdminOperationLogSearchOption option)
        {
            return dal.GetModelListByAjax_option(option);
        }
        #endregion
    }
}
