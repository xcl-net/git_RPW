using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPW.Model;

namespace RPW.Bll
{
    public partial class NewsBll
    {

        /// <summary>
        /// 删除一条数据(根据catid)
        /// </summary>
        public bool DeleteByCategoryId(long Id)
        {
            return dal.DeleteByCategoryId(Id);
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
            return dal.GetPageNewsList(catId, startRowNum, finishRowNum);
        }
    }
}
