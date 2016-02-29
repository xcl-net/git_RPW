using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class SegmentsBll
    {
        /// <summary>
        /// 得到一个 对象实体集合，从缓存中
        /// </summary>
        public List<RPW.Model.Segments> GetModelListByCache(string strWhere)
        {

            string CacheKey = "SegmentsModel-GetModelByCache" + strWhere;
            object objModel = Maticsoft.Common.DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = this.GetModelList(strWhere);
                    if (objModel != null)
                    {
                        int ModelCache = Maticsoft.Common.ConfigHelper.GetConfigInt("ModelCache");
                        Maticsoft.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    }
                }
                catch { }
            }
            return (List<RPW.Model.Segments>)objModel;
        }
    }
}
