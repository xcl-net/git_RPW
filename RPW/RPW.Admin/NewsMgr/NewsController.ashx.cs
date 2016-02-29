using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using RPW.Bll;
using RPW.Model;
using RPWRazor;
using System.IO;
using System.Configuration;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace RPW.Admin.NewsMgr
{
    /// <summary>
    /// NewsController 的摘要说明
    /// </summary>
    public class NewsController : BaseHandler
    {
        //新闻类别的“列表”
        public void list(HttpContext context)
        {
            //获取父类的 id
            string parentId = context.Request["parentId"];

            //首先，将父类级别默认设置为 0
            long newsParentId = 0;
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                newsParentId = Convert.ToInt64(parentId);
            }
            //查出来上级的类别名称
            var parentModel = new NewsCategoriesBll().GetModel(Convert.ToInt64(parentId));

            //加载列表模板
            var categories = new NewsCategoriesBll().GetModelList("ParentId=" + newsParentId);

            //查出来父类级别的根的id 是多少！
            if (newsParentId != 0)// 如果父类的根id不是0,
            {
                var cate = new NewsCategoriesBll().GetModel(newsParentId);
                //那就查出来，父类的根id
                long backId = cate.ParentId;

                RPRazorHelper.OutputRazor(context, "~/NewsMgr/CategoryList.cshtml",
                    new
                    {
                        categories = categories,
                        parentId = newsParentId,
                        backId = backId,
                        nameParent = parentModel.Name,
                        label = "当前类别是："
                    });
            }
            else //否则，父类的根id就是0，直接赋值 返回上级model值为0
            {
                RPRazorHelper.OutputRazor(context, "~/NewsMgr/CategoryList.cshtml",
                new { categories = categories, parentId = newsParentId, backId = 0, nameParent = "", label = "" });
            }

        }

        //新闻类别的“新增”
        public void addnew(HttpContext context)
        {
            //获取父类的 id
            string parentId = context.Request["parentId"];

            RPRazorHelper.OutputRazor(context, "~/NewsMgr/NewsAddnewEdit.cshtml",
                new { parentId = parentId, saveAction = "addnew", catName = "", catId = "" });
        }

        //新闻类别的“编辑”
        public void edit(HttpContext context)
        {
            //获取父类的 id
            string parentId = context.Request["parentId"];
            string catId = context.Request["id"];
            //根绝id查出这个model
            NewsCategories childModel = new NewsCategoriesBll().GetModel(Convert.ToInt64(catId));
            //加载
            RPRazorHelper.OutputRazor(context, "~/NewsMgr/NewsAddnewEdit.cshtml",
               new { parentId = parentId, saveAction = "edit", catName = childModel.Name, catId = catId });


        }
        //新闻类别的“保存”
        public void save(HttpContext context)
        {
            string saveAction = context.Request["saveAction"];
            //获取父类的 id
            string parentId = context.Request["parentId"];
            //获取类别的名字
            string catName = context.Request["catName"];

            //首先，将父类级别默认设置为 0
            long newsParentId = 0;      //就是加到了顶级新闻版块了
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                newsParentId = Convert.ToInt64(parentId);
            }
            if (string.IsNullOrWhiteSpace(catName))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "类别名称不能为空！");
                return;
            }
            if (catName.Length > 250)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "类别名称不能超过250个字符！");
                return;
            }

            //调用类别表
            NewsCategoriesBll catBll = new NewsCategoriesBll();

            if (saveAction == "addnew")
            {
                //实例一个model
                NewsCategories cat = new NewsCategories();
                //对model赋值
                cat.Name = catName;
                cat.ParentId = Convert.ToInt64(newsParentId);
                catBll.Add(cat);
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "新增成功！");
            }
            else if (saveAction == "edit")
            {
                string catId = context.Request["id"];
                //根绝类别id查出这个类别的model
                NewsCategories catModel = new NewsCategoriesBll().GetModel(Convert.ToInt64(catId));
                //对查出来的model，重新进行赋值
                catModel.Name = catName;
                catModel.ParentId = Convert.ToInt64(newsParentId);
                //执行更新
                catBll.Update(catModel);
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "修改成功！");
            }
            else
            {
                throw new Exception("saveAction错误！");
            }


        }
        //新闻类别的“删除”
        public void delete(HttpContext context)
        {
            string parentId = context.Request["parentId"];

            //获取要删除的新闻类别
            string catId = context.Request["id"];
            //看看这个类别的id，在新闻表T_News中是否，已经存在该类别新闻，先从新闻表中删除了，包含这个类别id的新闻，否则违反了约束，导致不能删除
            long catIIDD = Convert.ToInt64(catId);
            NewsBll newsBll = new NewsBll();
            newsBll.DeleteByCategoryId(catIIDD);

            //判断当前的类中，是否，有子类？有：不能删除；否则，可以删除！
            NewsCategoriesBll bll = new NewsCategoriesBll();
            if (bll.HasChildCategory(catIIDD))
            {
                //不给删除
                context.Response.Write("删除取消，原因：该类别下有子类别！");
                return;
            }
            else
            {
                //这时候，才开始删除T_NewsCatgories中的类别
                bll.Delete(catIIDD);
                context.Response.Redirect("NewsController.ashx?action=list&parentId=" + parentId);
            }
        }


        //新闻文章的“列表”
        public void newsList(HttpContext context)
        {
            //取得新闻类别的id
            long catId = Convert.ToInt64(context.Request["catId"]);

            //获取新闻所在的类别名称
            var catModel = new NewsCategoriesBll().GetModel(Convert.ToInt64(catId));
            string catName = catModel.Name;

            //查新闻表T_News
            //起始页码，末尾页码

            //这个类别下的文章的总条数---1
            NewsBll newsbll = new NewsBll();
            int totalSize = newsbll.GetRecordCount("CategoryId=" + catId);
            //每个新闻列表页的显示的多少条---2
            int sizePerPage = Convert.ToInt32(ConfigurationManager.AppSettings["NumPerPage"]);
            //新闻显示的总的页码数是---3
            int pageCount = (int)Math.Ceiling((totalSize * 1.0f) / (sizePerPage * 1.0f));


            //获取当前页的数据
            string s_pageNow = context.Request["pageNow"];
            int pageNow = 1;
            if (!string.IsNullOrWhiteSpace(s_pageNow))
            {
                pageNow = Convert.ToInt32(s_pageNow);
            }
            var newsItems = new NewsBll().GetPageNewsList(catId, (pageNow - 1) * sizePerPage + 1, pageNow * sizePerPage);



            #region 返回上级功能代码,以及加载模板
            //获取父类的 id
            string parentId = context.Request["catIdParentId"];

            //首先，将父类id级别默认设置为 0,就是顶级列表
            long newsParentId = 0;
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                newsParentId = Convert.ToInt64(parentId);
            }
            //查出来父类级别的根的id 是多少！
            if (newsParentId != 0)// 如果父类的id不是0,
            {
                //加载后台新闻文章标题列表模板
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/NewsMgr/NewsList.cshtml",
                    new
                    {
                        news = newsItems,
                        categoryId = catId,
                        catName = catName,
                        pagecount = pageCount,
                        backId = newsParentId

                    });
            }
            else
            {
                //加载后台新闻文章标题列表模板
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/NewsMgr/NewsList.cshtml",
                    new
                    {
                        news = newsItems,
                        categoryId = catId,
                        catName = catName,
                        pagecount = pageCount,
                        backId = 0

                    });

            }

            #endregion



        }



        //新闻文章的“新增”
        public void newsAddnew(HttpContext context)
        {
            //获取新闻所在的类别 id
            string catId = context.Request["catId"];
            var catModel = new NewsCategoriesBll().GetModel(Convert.ToInt64(catId));
            string catName = catModel.Name;

            RPRazorHelper.OutputRazor(context, "~/NewsMgr/NewsContentAddnewEdit.cshtml",
                new
                {
                    categoryId = catId,
                    saveAction = "newsAddnew",
                    content = "",
                    catName = catName,
                    title = "",
                    contentBody = "",
                    id = "",
                    time = "",

                });

        }
        //新闻文章的“编辑”
        public void newsEdit(HttpContext context)
        {

            //获取新闻所在的类别 id
            string catId = context.Request["catId"];
            var catModel = new NewsCategoriesBll().GetModel(Convert.ToInt64(catId));
            string catName = catModel.Name;
            //获取新闻的id
            long newsId = Convert.ToInt64(context.Request["id"]);
            //查出新闻的model
            var news = new NewsBll().GetModel(newsId);
            string title = news.Title;
            string content = news.Body;
            string timeCreate = news.PostDateTime.ToString();

            RPRazorHelper.OutputRazor(context, "~/NewsMgr/NewsContentAddnewEdit.cshtml",
                new
                {
                    categoryId = catId,
                    saveAction = "newsEdit",
                    content = "",
                    catName = catName,
                    title = title,
                    contentBody = content,
                    id = newsId,
                    time = timeCreate
                });
        }
        //新闻文章的“保存”
        public void newsSave(HttpContext context)
        {
            string saveAction = context.Request["saveAction"];
            //获取新闻所在的类别的 id
            string catId = context.Request["catId"];
            string title = context.Request["title"];
            string newsContent = context.Request["newsContent"];
            string id = context.Request["id"];
            //验证；
            if (string.IsNullOrWhiteSpace(title))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "新闻标题不能为空！");
                return;
            }
            if (title.Length > 250)
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "新闻标题不能超过250个字符！");
                return;
            }
            if (string.IsNullOrWhiteSpace(newsContent))
            {
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "error", "新闻内容不能为空！");
                return;
            }
            //新增保存
            if (saveAction == "newsAddnew")
            {
                //实例化一个新闻model
                News news = new News();
                news.CategoryId = Convert.ToInt64(catId);
                news.Title = title;
                news.PostDateTime = DateTime.Now;
                news.Body = newsContent;
                long newId = new NewsBll().Add(news);//新增，新闻，并且返回，新增新闻的id是多少？



                //静态化生成
                GenerateNewsStatic(newId, title, newsContent, catId);
                //每次新增完成后，都将前台的，新闻列表重新生成
                CreateNewsItemsStatic(Convert.ToInt64(catId));
                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "新增成功！");
            }
            else if (saveAction == "newsEdit")//编辑保存
            {
                //查出当期新闻的model
                //获取新闻的id
                long newsId = Convert.ToInt64(id);
                News news = new NewsBll().GetModel(newsId);
                news.CategoryId = Convert.ToInt64(catId);
                news.Title = title;
                news.PostDateTime = DateTime.Now;
                news.Body = newsContent;
                new NewsBll().Update(news);
                //每次修改完成后，都将新闻列表重新生成
                CreateNewsItemsStatic(Convert.ToInt64(catId));

                RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "修改成功！");

            }
            else
            {
                throw new Exception("saveAction错误！");
            }

        }
        //新闻文章的“删除”
        public void newsDelete(HttpContext context)
        {
            //获取新闻所在的类别的 id
            string cId = context.Request["catId"];
            long catId = Convert.ToInt64(cId);
            //要删除新闻的id
            string id = context.Request["id"];
            new NewsBll().Delete(Convert.ToInt64(id));
            //每次修改完成后，都将新闻列表重新生成
            CreateNewsItemsStatic(Convert.ToInt64(catId));

            context.Response.Redirect("NewsController.ashx?action=newsList&catId=" + catId);
        }

        //生成静态新闻页面方法
        /// <summary>
        /// 生成静态新闻页面方法
        /// </summary>
        /// <param name="newId">新闻id</param>
        /// <param name="title">新闻标题</param>
        /// <param name="body">新闻body内容</param>
        /// <param name="catId">新闻种类的id</param> 
        public static void GenerateNewsStatic(long newId, string title, string body, string catId)
        {

            #region 新闻新增完毕，生成，对新增的新闻，静态化处理
            //新闻类别名字
            var catModel = new NewsCategoriesBll().GetModel(Convert.ToInt64(catId));
            string catName = catModel.Name;

            string html = RPWRazor.RPRazorHelper.ParseRazor(HttpContext.Current, "~/NewsMgr/ViewNews.cshtml", new
            {
                newsId = newId,//新闻生成的时候，同时，将这个id新闻的二维码生成
                title = title,
                body = body,
                time = DateTime.Now,
                catgory = catName
            });
            //将生成的静态新闻，保存在哪里，前台！

            //从配置文件（web.config）中读取静态页生成路径
            string newStaticDir = ConfigurationManager.AppSettings["NewsStaticDir"];

            string fullPath = newStaticDir + catId + @"\" + newId + ".shtml";

            //判断这个新闻类别文件夹是否已经存在，不存在，就生成
            string htmlDir = Path.GetDirectoryName(fullPath);

            //如果文件夹不存在，则创建文件夹
            if (!Directory.Exists(htmlDir))
            {
                Directory.CreateDirectory(htmlDir);
            }

            //在指定的文件内写入，html文件
            File.WriteAllText(fullPath, html);

            //引用生成二维码方法，二维码生成的路径和静态新闻页放在了一个文件夹中
            CreateErWeiMa(newStaticDir, newId, catId);
            #endregion
        }


        //添加一个静态方法，用来生成二维码
        public static void CreateErWeiMa(string newsStaticDir, long newsId, string categoryId)
        {
            //二维码保存的路径
            string erWeiMaPath = Path.Combine(newsStaticDir, categoryId + "/" + newsId + ".png");


            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();

            string url = "http://www.rupeng.com/News/" + categoryId + "/" + newsId + ".shtml";
            qrEncoder.TryEncode(url, out qrCode);


            int ModuleSize = 6;//大小 
            QuietZoneModules QuietZones = QuietZoneModules.Two;  //空白区域  

            var render = new GraphicsRenderer(new FixedModuleSize(ModuleSize, QuietZones));

            using (System.IO.Stream stream = File.OpenWrite(erWeiMaPath))
            {
                render.WriteToStream(qrCode.Matrix, System.Drawing.Imaging.ImageFormat.Png, stream);
            }
        }

        //一键静态化处理
        public void rebuildStatic(HttpContext context)
        {
            //获取新闻所在的类别的 id
            string catId = context.Request["catId"];
            //查新闻表T_News
            var newss = new NewsBll().GetModelList("CategoryId=" + catId);
            foreach (var news in newss)
            {
                GenerateNewsStatic(news.Id, news.Title, news.Body, catId);
            }
            //每次重新生成后，都将新闻列表重新生成
            CreateNewsItemsStatic(Convert.ToInt64(catId));

            RPWCommonts.RPAjaxhelperCommons.WriteJson(context.Response, "ok", "静态化生成成功！");

        }

        //写个静态方法，读取模板文件 NewsItems.cshtml，用来生成，前台新闻文章列表的静态页面；
        private static void CreateNewsItemsStatic(long catId)
        {
            //获取新闻所在的类别类名
            var catModel = new NewsCategoriesBll().GetModel(catId);
            string catName = catModel.Name;

            //这个类别下的文章的总条数
            NewsBll newsbll = new NewsBll();
            int totalSize = newsbll.GetRecordCount("CategoryId=" + catId);
            //每个新闻列表页的显示的多少条
            int sizePerPage = Convert.ToInt32(ConfigurationManager.AppSettings["NumPerPage"]);
            //新闻显示的总的页码数是
            int pageCount = (int)Math.Ceiling((totalSize * 1.0f) / (sizePerPage * 1.0f));

            //循环调用，模板文件NewsItems.cshtml
            //生成新闻文章列表的静态页面,每次循环都会生成一个静态的页面
            for (int i = 1; i <= pageCount; i++)
            {
                //当前新闻列表页的，展示的内容是这些
                //
                //i=1;1--10条新闻
                //i=2;11--20条新闻
                List<News> newsItems = new NewsBll().GetPageNewsList(catId, (i - 1) * sizePerPage + 1, i * sizePerPage);

                //读取模板，生成新闻文章静态页
                string html = RPRazorHelper.ParseRazor(HttpContext.Current, "~/NewsMgr/NewsItems.cshtml",
                    new
                    {
                        CatgoryName = catName,
                        news = newsItems,
                        CatId = catId,
                        totalSize = totalSize,
                        NumPerPage = sizePerPage,
                        currentPage = i
                    }
                    );
                //到此，新闻文章列表的静态页面已经生成。
                //生成之后，现在要保存在磁盘上，这样，用户就能访问到新闻分页列表页面了，注意这些是系统生成的页面
                //使用IO
                //
                //新闻文章列表静态页保存的文件目录信息是
                string newsStaticDir = ConfigurationManager.AppSettings["NewsStaticDir"] + catId + @"\";
                //磁盘保存的路径是 反斜杠 ‘ \ ’

                //生成的新闻文章列表静态页每页的名称是
                string newsItemsStatic = "index_" + i + ".shtml";
                //获取生成这些文件的全路径
                string htmlFullPath = Path.Combine(newsStaticDir, newsItemsStatic);//文件夹、文件名

                //写入磁盘
                //判断文件夹是否存在
                string htmlDirectory = Path.GetDirectoryName(htmlFullPath);
                if (!Directory.Exists(htmlDirectory)) //文件夹不存在
                {
                    //创建
                    Directory.CreateDirectory(htmlDirectory);
                }
                File.WriteAllText(htmlFullPath, html);//向这个目录中，写入文本内容

            }
        }

    }
}