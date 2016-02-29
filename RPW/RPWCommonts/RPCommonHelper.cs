﻿using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RPWCommonts
{
    public class RPCommonHelper
    {
        /// <summary>
        /// 计算给定字符串的md5值，，可以从网上百度下来，这段代码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CalcMD5(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return CalcMD5(bytes);//减少了代码的复用
        }
        /// <summary>
        /// 计算给定字节流的md5值，根据上边的代码改造得到，并且做了代码的  复用的 设置
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string CalcMD5(byte[] bytes)
        {
            //执行与释放或重置非托管资源相关的应用程序定义的任务。MD5CryptoServiceProvider实现了IDisposeable接口
            using (MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider())
            {
                StringBuilder builder = new StringBuilder();

                bytes = provider.ComputeHash(bytes);

                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2").ToLower());

                return builder.ToString();
            }
        }


        #region Des加密算法

        public static string _KEY = "HQDCKEYX";  //密钥
        public static string _IV = "HQDCKEY2";   //向量 保证都是8位数字。

        //Des内部实现不必太关心，会使用，达到其功能就行了。

        /// <summary>
        /// Des加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DesEncypt(string data)
        {

            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);

            using (DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider())
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cst))
            {
                int i = cryptoProvider.KeySize;
                sw.Write(data);
                sw.Flush();
                cst.FlushFinalBlock();
                sw.Flush();

                string strRet = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
                return strRet;
            }

        }

        /// <summary>
        /// Des解密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string DesDecrypt(string data)
        {

            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);

            byte[] byEnc;

            try//密码解密的过程，可能会，抛异常：因为cookie 信息被软件改掉，用户修改，对解密失败，进行捕获异常！
            {
                data.Replace("_%_", "/");
                data.Replace("-%-", "#");
                byEnc = Convert.FromBase64String(data);

            }
            catch
            {
                return null;//解密失败的cookie返回为空
            }

            using (DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider())
            using (MemoryStream ms = new MemoryStream(byEnc))
            using (CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cst))
            {
                return sr.ReadToEnd();
            }
        }
        #endregion

        //设置一个常量
        public const string VALIDCODE = "validcode";

        //随机四个汉字写在画布上，生成验证码
        public static void GenerateValidCode(HttpContext context)
        {
            context.Response.ContentType = "image/jpeg";//1.1 

            //1.9 在这里调用，ResetYZM ，生成四个随机的汉字，并且存到session中
            string yzm = ResetYZM(context);

            /*
            //1.8 将yzm存入到session中，
            context.Session[Login.VERIFICATIONCODE] = yzm;// 分析：这一句应该放到ResetYZM（）方法中。不然，错误后，重新刷新过的雁阵吗，并没有放到Session中
            */

            //`1.6 画上汉字
            using (Bitmap bmp = new Bitmap(120, 30))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                using (Font font = new Font(new FontFamily("隶书"), 18))
                {
                    g.Clear(Color.Pink);
                    g.DrawString(yzm, font, Brushes.Red, new PointF(0, 0));
                    Random rand = new Random();
                    //1.7 画上一些点，影响识别
                    for (int i = 0; i < 80; i++)//画上80个随机的点
                    {
                        int x = rand.Next(0, 120);
                        int y = rand.Next(0, 30);
                        g.DrawLine(Pens.Red, x, y, x + 1, y + 1);
                    }
                }
                bmp.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            }
        }

        //重置验证码中的汉字内容
        public static string ResetYZM(HttpContext context)
        {
            //1.2 常用汉字字符串
            string cyhz = "人口手大小多少上中下男女天地会反清复明杨中科小宝双儿命名空间语现在明天来多个的我山东河北南固安北京南昌东海西安是沙河高教园学"
                + "木禾上下土个八入大天人火文六七儿九无口日中了子门月不开四五目耳头米见白田电也长山出飞马鸟云公车牛羊小少巾牙尺毛又心手水广升足"
                + "走方半巴业本平书自已东西回片皮生里果几用鱼今正雨两瓜衣来年左右万百丁齐冬说友话春朋高你绿们花红草爷亲节的岁行古处声知多忙洗真认父扫"
                + "母爸写全完关家看笑着兴画会妈合奶放午收女气太早去亮和李语秀千香听远唱定连向以更后意主总先起干明赶净同专工才级队蚂蚁前房空网诗黄林闭"
                + "童立是我朵叶美机她过他时送让吗往吧得虫很河借姐呢呀哪谁凉怕量跟最园脸因阳为光可法石找办许别那到都吓叫再做象点像照沙海桥军竹苗井面乡"
                + "忘想念王这从进边道贝男原爱虾跑吹乐地老快师短淡对热冷情拉活把种给吃练学习非苦常问伴间共伙汽分要没孩位选北湖南秋江只帮星请雪就球跳玩"
                + "桃树刚兰座各带坐急名发成动晚新有么在变什条";


            //1.3 用随机数取得 四个汉字的字符串
            //1.4 声明一个字符串,先为空
            string yzm = "";
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                int index = rand.Next(0, cyhz.Length);//1.5 这里使用Length
                yzm += cyhz[index];//字符串叠加                
            }
            context.Session[VALIDCODE] = yzm;
            return yzm;
        }

        //获取验证码从session中
        public static string GetValidCode(HttpContext context)
        {
            return (string)context.Session[VALIDCODE];
        }
    }
}
