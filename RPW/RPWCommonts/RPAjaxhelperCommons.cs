using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace RPWCommonts
{
    public class RPAjaxhelperCommons
    {
        //1.1 这个类主要是返回客户端Json字符串的
        //添加JavaScriptSerializer的程序集应用


        public static void WriteJson(HttpResponse response, string status, string msg, object data = null)//data默认值是空
        {
            //1.2 返回类型，修改为json
            response.ContentType = "application/json";
            //1.3 创建一个匿名对象，方便序列化
            var obj = new { Status = status, Msg = msg, Data = data };
            //1.4 序列化
            string json = new JavaScriptSerializer().Serialize(obj);
            response.Write(json);
        }

    }
}
