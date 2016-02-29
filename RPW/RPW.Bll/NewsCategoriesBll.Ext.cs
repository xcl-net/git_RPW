using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class NewsCategoriesBll
    {
        /// <summary>
        /// 判断是存在子类别，返回真，就是有子类
        /// </summary>
        /// <param name="catId"></param>
        /// <returns></returns>
        public bool HasChildCategory(long catId)
        {
            return dal.HasChildCategory(catId);
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public RPW.Model.NewsCategories GetModelByParentId(long ParentId)
        {
            return dal.GetModelByParentId(ParentId);
        }
    }
}
