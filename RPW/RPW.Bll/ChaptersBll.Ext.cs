using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class ChaptersBll
    {
        /// <summary>
        /// 得到  一个  对象实体的集合，从缓存中
        /// </summary>
        public List<RPW.Model.Chapters> GetModelListByCache(string strWhere)
        {

            string CacheKey = "ChaptersModel-GetModelListByCache" + strWhere; //缓存的id

            object objModel = Maticsoft.Common.DataCache.GetCache(CacheKey);  //设置一个缓存

            if (objModel == null)  //如果缓存为空
            {
                objModel = this.GetModelList(strWhere); //查询出数据实体集合，存入到缓存中

                if (objModel != null)  //缓存不为空
                {
                    int ModelCache = Maticsoft.Common.ConfigHelper.GetConfigInt("ModelCache"); //设置缓存时间
                    Maticsoft.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    // 缓存id     缓存实体对象    过期时间（从当前时间加分钟）  时间跨度
                }

            }
            return (List<RPW.Model.Chapters>)objModel;
        }
    }
}
