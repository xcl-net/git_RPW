using RazorEngine;
using RazorEngine.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RPWRazor
{
    public class RPRazorHelper
    {
        //1.封装一个方法，省去了每次都重复自己添加cacheName的麻烦
        public static string ParseRazor(HttpContext context, string csHtmlVirtualPath, object model = null)
        {
            //2.拿到虚拟路径
            string fullPath = context.Server.MapPath(csHtmlVirtualPath);
            //3.读取模板
            string cshtml = File.ReadAllText(fullPath);
            //4.给模板文件取一个别名字
            string cacheName = fullPath + File.GetLastWriteTime(fullPath);
            //5.用model替换变量
            string html = Razor.Parse(cshtml, model, cacheName);
            //6.返回模板文件内容
            return html;
        }
        /// <summary>
        /// 如果调用RPRazorHelper中的ParseRazor方法，就可以用这个方法简化，省去了context.response.write();
        /// </summary>
        /// <param name="context"></param>
        /// <param name="csHtmlVirtualPath"></param>
        /// <param name="model"></param>
        public static void OutputRazor(HttpContext context, string csHtmlVirtualPath, object model = null)
        {
            string html = ParseRazor(context, csHtmlVirtualPath, model);
            context.Response.Write(html);

        }

        /// <summary>
        /// 使得传递进去的字符串都是按照原样输出到浏览器中执行
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static RawString Raw(string str)
        {
            return new RawString(str);
        }

        /// <summary>
        /// 加载 html 内容到cshtml中
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static RawString Include(string virtualPath)
        {

            //获得要加载的html 文件的物理路径
            string filePath = HttpContext.Current.Server.MapPath(virtualPath);
            string html = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            return Raw(html);
        }



        public static RawString CheckBox(bool isChecked, object extProperties = null)//标签有名字，id，和是否选中
        {
            //1.new一个StringBuilder
            StringBuilder ss = new StringBuilder();
            //2.拼接一个CheckBox方法
            ss.Append("<input type='checkbox' ");

            if (extProperties != null)//获取id。name。style等属性
            {
                ss.Append(RenderExtendPeorperties(extProperties));
            }

            if (isChecked)//如果是选中的
            {
                ss.Append("checked");
            }
            ss.Append("/>");
            return new RawString(ss.ToString());//返回生成的标签
        }


        /// <summary>
        /// 下拉列表，属性只有一个参数id的
        /// </summary>
        /// <param name="items"></param>
        /// <param name="valuePropName"></param>
        /// <param name="textPropName"></param>
        /// <param name="selectedValue"></param>
        /// <param name="extenedProperties"></param>
        /// <returns></returns>
        public static RawString DropDownList(IEnumerable items, string valuePropName, string textPropName, IEnumerable selectedValue, object extenedProperties = null)//1-1.这里extenedProperties指的就是匿名类名字，它也是属于objec类型的
        {

            StringBuilder sb = new StringBuilder();

            //1.1 拼接出来 “select”标签----------------这里一堆代码主要是 拼接 关于select 的属性集合
            sb.Append("<select");
            //用反射类Type调用程序集中的方法

            /*     此代码提取成一个方法，，，RenderExtendPeorperties（）
            Type extendPropertiesType = extenedProperties.GetType();//1-2.既然是类就可以反射拿到类的名字，实例化创建对象；
            PropertyInfo[] extPropInfos = extendPropertiesType.GetProperties();//1-3.反射获取类中的属性；


            foreach (var extPropInfo in extPropInfos)//1-4.所以这里就可以遍历
            {
                string extProName = extPropInfo.Name;//1-5.取得属性的名字
                object extPropValue = extPropInfo.GetValue(extenedProperties);
                sb.Append(" ").Append(extProName).Append("='").Append(extPropValue).Append("'");

            }
             */

            if (extenedProperties != null)//获取id name style..等等属性
            {
                sb.Append(RenderExtendPeorperties(extenedProperties));
            }
            sb.Append(">");

            //2.2 拼接出来 “option” 标签----------------这里一堆代码主要是 拼接 关于option 的属性集合
            foreach (Object item in items)
            {
                Type itemType = item.GetType();//获得对象类型的名字

                PropertyInfo valuePropInfo = itemType.GetProperty(valuePropName);//拿到valuePropName("Id")的属性
                object itemValueValue = valuePropInfo.GetValue(item);//获得就是item对象的“Id”的值

                PropertyInfo textPropInfo = itemType.GetProperty(textPropName);//拿到“Name”的属性
                object itemTextValue = textPropInfo.GetValue(item);//拿到item的“Name”属性的值

                //等于selectedValue的项增加一个“selecte”属性，它被选中
                sb.Append("<option value='").Append(itemValueValue).Append("'");

                /* if (Object.Equals(itemValueValue, selectedValue))  //如果selectedValue类型是object类型，则启用注释的此部分代码
                {
                    sb.Append("selected");
                }  */

                //1---.3如果是选中的就加上 selected='selected'
                if (Contains(selectedValue, itemValueValue))
                {
                    sb.Append(" ").Append("selected='selected'");//效果：<input type="checkbox" name="manager" checked  />属性，它被选中
                }

                sb.Append(" ").Append("/>");

                sb.Append(itemTextValue).Append("</option>");
            }

            sb.Append("</select>");
            return new RawString(sb.ToString());


        }


        /// <summary>
        /// 生成checkBoxList标签
        /// </summary>
        /// <param name="items">要绑定数据的集合</param>
        /// <param name="valuePropName">绑定数据类的Id属性</param>
        /// <param name="textPropName">绑定数据类的Name属性</param>
        /// <param name="selectedValue">指定默认选中的项，一般通过Model传递过来</param>
        /// <param name="extendProPerties">标签的扩展属性，一般通过匿名类传递过来，特别注意：name属性，
        /// 需要放在第一个位置，name属性决定了id属性</param>
        /// <returns></returns>
        public static RawString CheckBoxListOrRadioList(IEnumerable items,
            string valuePropName, string textPropName, IEnumerable selectedValue, object extendProPerties)
        {
            StringBuilder sb = new StringBuilder();


            //利用反射Type调用程序集中的匿名类的 名字
            Type extendPropertiesType = extendProPerties.GetType();
            //反射获取匿名类中的所有属性
            PropertyInfo[] extPropInfos = extendPropertiesType.GetProperties();


            //2.2拼接出来<label>标签
            //items是传递过来的list集合（就是要绑定数据类的集合），，item是集合中的一个类
            foreach (object item in items)
            {
                //1.1拼接出<input >
                sb.Append("<input ");

                //1---.1渲染出来<input type="checkbox" name="manager" style="..." />中的
                //type="checkbox" name="manager" style="..."属性
                foreach (var extPropInfo in extPropInfos)
                {
                    //获取属性的名字
                    string extProName = extPropInfo.Name;
                    //获取属性对应的值
                    object extProValue = extPropInfo.GetValue(extendProPerties);
                    //名字=值
                    sb.Append(" ").Append(extProName).Append("='").Append(extProValue).Append("'");

                }
                //由匿名类传递到这个CheckBoxList方法的属性，渲染完毕!

                //开始绑定数据了。
                //1---.2拿到<input type="checkbox" name="manager" style=... />中的   value="1"   属性
                //value因为需要绑定数据（动态），故没有放在匿名类中传递
                //获取item的类名字（要绑定数据的类名字）
                Type itemType = item.GetType();

                //获取item表示类（要绑定数据的类）中指定属性的 <<名字>>；
                //valuePropName用来指定，要绑定类中哪个属性（名字叫“Id”的那个）
                PropertyInfo valuePropInfo = itemType.GetProperty(valuePropName);

                //获得item表示的类（要绑定数据的类）中指定属性“Id”的<<值>>
                object itemValue = valuePropInfo.GetValue(item);

                //从item类（要绑定数据的类）获取“键”=“值”完毕，渲染到标签 "value"属性中，（value值才能传递到服务器中）
                sb.Append(" ").Append("value").Append("='").Append(itemValue.ToString()).Append("' ");

                //渲染标签id属性
                //问题：如果是在同一个页面同时调用 CheckBoxList（）方法，为了使id值不相同，id值一定要取一个不能重复的别名
                //取当前标签的name属性对应的值 + 绑定数据的id值 = 做为id的名字
                string idName = extPropInfos[0].GetValue(extendProPerties) + itemValue.ToString();
                sb.Append(" ").Append("id").Append("='").Append(idName).Append("' ");

                //1---.3如果是选中的就加上 "checked"
                if (Contains(selectedValue, itemValue))
                {
                    sb.Append(" ").Append("checked");//效果：<input type="checkbox" name="manager" checked  />属性，它被选中
                }

                sb.Append(" ").Append("/>");

                //2.2渲染（要绑定数据类）中的Name属性
                //获取item的类（要绑定数据的类）中指定属性的 <名字>；
                //textPropName用来指定要绑定类中哪个属性（名字叫“Name”的那个）
                PropertyInfo textPropInfo = itemType.GetProperty(textPropName);
                object itemTextValue = textPropInfo.GetValue(item);//获得item表示的类中指定属性“Name”的<值>

                //同时渲染<label>标签的属性，目的：为了使用方便，加for 为哪个id
                sb.Append("<label for='").Append(idName).Append("'>").Append(itemTextValue).Append("</label>");
                sb.Append("<br />");


            }
            return new RawString(sb.ToString());
        }

        /// <summary>
        /// 判断itemValue是否存在于selectedValue中(一个角色选中的权限Id，在权限列表中进行比较,如果是相等，就返回True)
        /// 判断数据中的id值，是否在选中的集合【数组】中
        /// </summary>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool Contains(IEnumerable selectedValue, object itemValue)
        {
            foreach (object item in selectedValue)
            {
                if (object.Equals(itemValue, item))
                {
                    return true;
                }
            }
            return false;
        }


        public static string RenderExtendPeorperties(object extendProperties)
        {
            StringBuilder sb = new StringBuilder();
            Type extendPropertiesType = extendProperties.GetType();
            PropertyInfo[] extPropInfos = extendPropertiesType.GetProperties();//获得类型的属性
            foreach (var extPronInfo in extPropInfos)
            {
                string extPropName = extPronInfo.Name;//属性名
                object extPropValue = extPronInfo.GetValue(extendProperties);//属性值
                sb.Append(" ").Append(extPropName).Append("='").Append(extPropValue).Append("'");

            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成分页组件
        /// </summary>
        /// <param name="urlFormat">超链接的格式</param>
        /// <param name="totalSize">总的数据条数</param>
        /// <param name="pageSize">每页多少条</param>
        /// <param name="currentPage">当前页的页码</param>
        /// <returns></returns>
        public static RawString Paging(string urlFormat, long totalSize, long pageSize, long currentPage)
        {
            StringBuilder sb = new StringBuilder();
            //实现的效果：
            //currentPage表示当前页：前边显示5页，后边显示5页：


            //[首页]1,2,3,{4},5,6,7,8,9[尾页]   -----------前边显示不到5个页码，怎么实现这个效果？

            //[首页]3,4,5,6,7,{8},9,10 [尾页]   -----------后边显示不到5个页码，怎么实现这个效果？    

            //[首页]3,4,5,6,7,{8},9,10,11,12,13 [尾页]   --正常显示...

            //计算页码的总页数: 一共51页，每页10条，则需要5.1页，就是取天花板数，6页！
            //                    一共50页，每页10条，则需要5页！
            //                     一共48页，每页10条，则需要5页！

            //                   总结：取得都是，  天花板数

            long totalPageCount = (long)Math.Ceiling((totalSize * 1.0f) / pageSize);//一个是浮点数就行，测试得到。

            //在当期页面前后，各 最多显示 5个，页码

            //计算页码中的第一个页码
            long firstPageNum = Math.Max(currentPage - 5, 1);
            //注释：当是这个情况时候：[首页]1,2,3,{4},5,6,7,8,9[尾页]   4-5=-1   取最大值，那么第一页显示的就是 1  ，也就是第一页
            //      当是这个情况时候：[首页]3,4,5,6,7,{8},9,10 [尾页]    8-5=3   取最大值，那么第一页显示的就是3   ，正好符合

            //计算页码中的最后一个页码
            long lastPageNum = Math.Min(currentPage + 5, totalPageCount);
            //注释：当是这个情况时候：[首页]3,4,5,6,7,{8},9,10 [尾页]  8+5=13  取最小值，那么最后一页显示的就是10   就是尾页

            //拼接出来，分页组件

            //如果当前页是 第1页，只是就显示“尾页”“下一页”;
            if (currentPage == 1)
            {
                for (long i = firstPageNum; i <= lastPageNum; i++)
                {
                    string url = urlFormat.Replace("{pageNum}", i.ToString());
                    if (i == currentPage)
                    {
                        sb.Append("<li class='active'><a>" + i + "</a></li>");
                    }
                    else
                    {
                        sb.Append("<li><a href='" + url + "'>" + i + "</a></li>");
                    }
                }
                sb.AppendLine("<li><a href='" + urlFormat.Replace("{pageNum}", (currentPage + 1).ToString()) + "'>下一页</a></li>");
                sb.AppendLine("<li><a href='" + urlFormat.Replace("{pageNum}", totalPageCount.ToString()) + "'>尾页</a></li>");
            }
            //如果当前页是 第最后页，只是就显示“首页”“上一页”;
            else if (currentPage == lastPageNum)
            {
                sb.AppendLine("<li><a href='" + urlFormat.Replace("{pageNum}", "1") + "'>首页</a></li>");
                sb.AppendLine("<li><a href='" + urlFormat.Replace("{pageNum}", (currentPage - 1).ToString()) + "'>上一页</a></li>");
                for (long i = firstPageNum; i <= lastPageNum; i++)
                {
                    string url = urlFormat.Replace("{pageNum}", i.ToString());
                    if (i == currentPage)
                    {
                        sb.Append("<li class='active'><a>" + i + "</a></li>");
                    }
                    else
                    {
                        sb.Append("<li><a href='" + url + "'>" + i + "</a></li>");
                    }
                }
            }
            //如果当前页不是第一页，也不是最后一页，就显示“首页”，“上一页”，“下一页”，“尾页”;
            else
            {

                sb.AppendLine("<li><a href='" + urlFormat.Replace("{pageNum}", "1") + "'>首页</a></li>");
                sb.AppendLine("<li><a href='" + urlFormat.Replace("{pageNum}", (currentPage - 1).ToString()) + "'>上一页</a></li>");
                for (long i = firstPageNum; i <= lastPageNum; i++)
                {
                    string url = urlFormat.Replace("{pageNum}", i.ToString());
                    if (i == currentPage)
                    {
                        sb.Append("<li class='active'><a>" + i + "</a></li>");
                    }
                    else
                    {
                        sb.Append("<li><a href='" + url + "'>" + i + "</a></li>");
                    }
                }
                sb.AppendLine("<li><a href='" + urlFormat.Replace("{pageNum}", (currentPage + 1).ToString()) + "'>下一页</a></li>");
                sb.AppendLine("<li><a href='" + urlFormat.Replace("{pageNum}", totalPageCount.ToString()) + "'>尾页</a></li>");

            }

            return new RawString(sb.ToString());


        }


    }
}
