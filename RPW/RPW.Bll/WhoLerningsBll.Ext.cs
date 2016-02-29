using RPW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class WhoLerningsBll
    {
        /// <summary>
        /// 获得前几行数据,改造的动软的方法
        /// </summary>
        public List<WhoLernings> GetListShowTopByCache(int Top, string strWhere, string filedOrder)
        {
            //对业务逻辑层，使用缓存处理
            string cacheKey = "GetListShowTop--" + Top; //给缓存 取一个标签名字（保证不重复就行）
            object objModel = Maticsoft.Common.DataCache.GetCache(cacheKey);//设置一个缓存（相当于一个空的缓存）

            if (objModel == null)//如果缓存为空的话
            {
                try
                {
                    objModel = dal.GetListShowTop(Top, strWhere, filedOrder);//将查询出来的 list<WhoLernings> 对象放到缓存中
                    if (objModel != null)
                    {
                        int modelCache = Maticsoft.Common.ConfigHelper.GetConfigInt("ModelCacheRecords");//缓存时间
                        Maticsoft.Common.DataCache.SetCache(cacheKey, objModel, DateTime.Now.AddMinutes(modelCache), TimeSpan.Zero);
                    }
                }
                catch (Exception ex)
                {

                }
            }


            return (List<WhoLernings>)objModel;//强制类型转换   //不为空，直接返回对象List<WhoLernings>
        }


    }
}
