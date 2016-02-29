using RPW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class ActiveCodesBll
    {
        /// <summary>
        /// 根据用户名，查激活码
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActiveCodes GetCodeByName(string name)
        {
            return dal.GetCodeByName(name);
        }


        /// <summary>
        /// 写个判断时间的函数,分钟查为分钟
        /// </summary>
        /// <param name="zhuceTime"></param>
        /// <param name="jihuoTime"></param>
        /// <returns></returns>
        public long TimeValue(string zhuceTime, string jihuoTime)
        {
            return dal.TimeValue(zhuceTime, jihuoTime);
        }
    }
}
