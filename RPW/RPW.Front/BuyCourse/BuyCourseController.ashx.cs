using RPW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RPW.Bll;
using System.Configuration;

namespace RPW.Front.BuyCourse
{
    /// <summary>
    /// BuyCourseController 的摘要说明
    /// </summary>
    public class BuyCourseController : BaseHandler
    {
        //写入到配置文件中
        private static string AliPayPartner = ConfigurationManager.AppSettings["AliPayPartner"];//在支付宝商店编号
        private static string AliPaySellerEmail = ConfigurationManager.AppSettings["AliPaySellerEmail"];//卖家地址
        private static string AliPayKey = ConfigurationManager.AppSettings["AliPayKey"];//在支付宝中商户密钥
        private static string WangYinPartner = ConfigurationManager.AppSettings["WangYinPartner"];//在网银中商店编号
        private static string WangYinKey = ConfigurationManager.AppSettings["WangYinKey"];//在网银中商户密钥


        //去支付选择界面，购买课程
        public void BuyCourse(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            //要购买的课程id
            long courseId = Convert.ToInt64(context.Request["courseId"]);
            //要购买的用户的id
            long? userId = FrontHelper.GetUserIdInSession(context);
            //可能情况：当前没有用户登录：
            if (userId == null)
            {
                //指定到“登录界面，并返回”
                context.Response.Redirect("/login.shtml");
                return;
            }

            //根据课程id，查询课程的价格
            CoursesBll courseBll = new CoursesBll();
            Courses course = courseBll.GetModel(courseId);
            //课程的价格
            int? price = course.Price;
            if (price == null || price <= 0)
            {
                FrontHelper.OutPutMsg(context, "本课程免费，不需要购买");
                return;
            }

            //否则，课程是收费的
            //创建订单
            CourseOrdersBll courseOrderBll = new CourseOrdersBll();
            long orderId = courseOrderBll.AddOrder((long)userId, courseId);//走到这里，用户id是一定存在的

            //前往支付

            #region 发链接给，支付宝系统，参数
            //商品编号（商户与支付宝合作后在支付宝产生的用户 ID）
            string partner = AliPayPartner;
            //回调商户地址（通过商户网站的哪个页面来通知支付成功！） 
            string return_url = "http://localhost:23773/BuyCourse/BuyCourseController.ashx?action=PayReturn";
            //商品名称
            string subject = course.CName;
            //商品描述
            string body = course.DescriptionCourses;
            //订单号！！！(由商户网站生成，支付宝不确保正确性，只负责转发。) 
            string out_trade_no = orderId.ToString();//订单号id做为订单号
            //总金额 
            string total_fee = course.Price.ToString();
            //卖家邮箱 
            string seller_email = AliPaySellerEmail;
            //在支付宝中的商户密钥
            string key = AliPayKey;//"abc123";
            //总金额、 商户编号、订单号、商品名称、商户密钥的MD5值
            string sign = RPWCommonts.RPCommonHelper.CalcMD5(total_fee + partner + out_trade_no + subject + key);
            #endregion

            #region 发链接给，网银系统，参数
            //在网银中商店编号
            string v_mid = WangYinPartner;
            //订单编号
            string v_oid = orderId.ToString();
            //价格
            string v_amount = course.Price.ToString();
            //货币类型
            string v_moneytype = "0";
            //（网银）回调商户地址
            string v_url = "http://localhost:23773/BuyCourse/BuyCourseController.ashx?action=PayReturnByWY";
            //在网银在线中的商户密钥
            string wykey = WangYinKey;//"123456";
            // 数字签名。为按顺序连接:  总金额、币种、订单号、商户编号、商户密钥为新字符串的MD5值。
            string v_md5info = RPWCommonts.RPCommonHelper.CalcMD5(v_amount + v_moneytype + v_oid + v_mid + wykey);
            #endregion

            #region 加载付款方式页面

            RPWRazor.RPRazorHelper.OutputRazor(context, "/BuyCourse/PayOpt.cshtml",
                new
                {
                    //支付宝
                    partner = partner,
                    return_url = context.Server.UrlEncode(return_url),
                    subject = context.Server.UrlEncode(subject),
                    body = context.Server.UrlEncode(body),
                    out_trade_no = out_trade_no,
                    total_fee = total_fee,
                    seller_email = context.Server.UrlEncode(seller_email),
                    sign = sign
                    ,
                    //网银在线
                    v_mid__ = v_mid,//网银商户编号
                    v_oid__ = v_oid,
                    v_amount__ = v_amount,
                    v_moneytype__ = v_moneytype,
                    v_url__ = context.Server.UrlEncode(v_url),
                    v_md5info__ = v_md5info,
                    style = 2, //网关模式：0(普通列表)，2(银行列表中带外卡)
                    remark1 = context.Server.UrlEncode(""),//传递的备注1
                    remark2 = context.Server.UrlEncode("")
                }
                );

            #endregion

        }
        //去支付宝付款
        public void AliPay(HttpContext context)
        {
            //传递给支付宝系统的必要参数
            string partner = context.Request["partner"];
            string return_url = context.Request["return_url"];
            string subject = context.Request["subject"];
            string body = context.Request["body"];
            string out_trade_no = context.Request["out_trade_no"];
            string total_fee = context.Request["total_fee"];
            string seller_email = context.Request["seller_email"];
            string sign = context.Request["sign"];



            context.Response.Redirect(
              "http://paytest.rupeng.cn/AliPay/PayGate.ashx?partner=" + partner
              + "&return_url=" + return_url
              + "&subject=" + subject
              + "&body=" + body
              + "&out_trade_no=" + out_trade_no
              + "&total_fee=" + total_fee
              + "&seller_email=" + seller_email
              + "&sign=" + sign);
        }

        //去网银在线付款
        public void WanYinPay(HttpContext context)
        {
            //传递给网银在线系统的必要参数
            string v_mid = context.Request["v_mid"];
            string v_oid = context.Request["v_oid"];
            string v_amount = context.Request["v_amount"];
            string v_moneytype = context.Request["v_moneytype"];
            string v_url = context.Request["v_url"];
            string v_md5info = context.Request["v_md5info"];
            string style = context.Request["style"];
            string remark1 = context.Request["remark1"];
            string remark2 = context.Request["remark2"];


            context.Response.Redirect(
              "http://paytest.rupeng.cn/ChinaBank/PayGate.ashx?v_mid="
              + v_mid
              + "&v_oid=" + v_oid
              + "&v_amount=" + v_amount
              + "&v_moneytype=" + v_moneytype
              + "&v_url=" + v_url
              + "&v_md5info=" + v_md5info
              + "&style=" + style
              + "&remark1=" + remark1 + "&remark2=" + remark2);


        }

        //支付宝系统返回给商户收款结果
        public void PayReturn(HttpContext context)
        {
            //返回的订单编号
            string out_trade_no = context.Request["out_trade_no"];
            //返回码，字符串。ok为支付成功，error为支付失败。
            string returncode = context.Request["returncode"];
            //支付金额
            string total_fee = context.Request["total_fee"];
            //支付宝返回给商户网站的签名
            string sign = context.Request["sign"];
            //按顺序链接：订单号、返回码、支付金额、商户密钥为新字符串的MD5值
            string mysign = RPWCommonts.RPCommonHelper.CalcMD5(out_trade_no + returncode + total_fee + "abc123");

            //如果商户自己计算的md5与支付宝系统给的签名sign不一致
            if (sign != mysign)
            {
                FrontHelper.OutPutError(context, "数据校验失败!");
                return;
            }
            if (returncode == "ok")
            {
                //支付成功

                //获取订单号,(订单号，作为了商品编号)
                long orderId = Convert.ToInt64(out_trade_no);
                //查出订单信息
                CourseOrders order = new CourseOrdersBll().GetModel(orderId);
                if (order == null)//(编程要严谨！！！)
                {
                    FrontHelper.OutPutMsg(context, "根据订单号没有找到订单信息，非常抱歉，您的资金很安全，请联系我们的。。。。");
                    return;
                }
                //如果做一个在线手机充值的功能
                //如果用户获得了请求的格式，然后重复的向payReturn发送请求，
                //那么订单就会重复确认，商户系统就会给用户多次充值
                if (order.IsPayed)//
                {
                    FrontHelper.OutPutError(context, "订单已经支付，不需要重复确认！");
                    return;
                }

                //两个检查，完后
                order.IsPayed = true;//说明：顾客支付过钱了,商家可以发货了
                order.PayDateTime = DateTime.Now;

                new CourseOrdersBll().Update(order);
                //信守承诺，下订单的时候课程是什么价格就是什么价格，因为刚下了订单后可能课程就涨价了
                //因此不做价格检查

                //发货
                //给 "前台用户--课程" 表，添加新的记录
                UserCourses userCourse = new UserCourses();
                userCourse.CourseId = order.CourseId;//购买的课程id
                userCourse.ExpireDateTime = DateTime.Now.AddYears(1);//课程有效期一年
                userCourse.UserId = order.UserId;
                UserCoursesBll userCourseBll = new UserCoursesBll();
                userCourseBll.Add(userCourse);
                long courseId = order.CourseId;
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/BuyCourse/BuyCourseOk.cshtml", courseId);

            }
            else
            {
                FrontHelper.OutPutError(context, "系统提示,支付失败!");
            }
        }

        //网银系统返回给商户收款结果
        public void PayReturnByWY(HttpContext context)
        {
            //订单号
            string v_oid = context.Request["v_oid"];
            //支付银行。目前值衡为0.
            string v_pmode = context.Request["v_pmode"];
            //支付结果。20为成功，30为支付失败
            string v_pstatus = context.Request["v_pstatus"];
            //总金额
            string v_amount = context.Request["v_amount"];
            //币种。0为人民币，1为外币。
            string v_moneytype = context.Request["v_moneytype"];
            //传递的备注1。
            string remark1 = context.Request["remark1"];
            //传递的备注2
            string remark2 = context.Request["remark2"];
            //网银发给商户系统新的md5值；
            string v_md5str = context.Request["v_md5str"];
            //数字签名。为按顺序连接  订单号、支付结果、总金额、币种、商户密钥为新字符串的MD5值。
            string my_v_md5str = RPWCommonts.RPCommonHelper.CalcMD5(v_oid + v_pstatus + v_amount + v_moneytype + "123456");
            if (v_md5str != my_v_md5str)
            {
                context.Response.Write("数据校验失败！");
                return;
            }
            if (v_pstatus == "20")
            {
                //context.Response.Write("支付成功！");
                //支付成功
                #region 和支付宝支付成功，处理结果一样；

                //获取订单号,(订单号，作为了商品编号)
                long orderId = Convert.ToInt64(v_oid);
                //查出订单信息
                CourseOrders order = new CourseOrdersBll().GetModel(orderId);
                if (order == null)//(编程要严谨！！！)
                {
                    FrontHelper.OutPutMsg(context, "根据订单号没有找到订单信息，非常抱歉，您的资金很安全，请联系我们的。。。。");
                    return;
                }
                //如果做一个在线手机充值的功能
                //如果用户获得了请求的格式，然后重复的向payReturn发送请求，
                //那么订单就会重复确认，商户系统就会给用户多次充值
                if (order.IsPayed)//
                {
                    FrontHelper.OutPutError(context, "订单已经支付，不需要重复确认！");
                    return;
                }

                //两个检查，完后
                order.IsPayed = true;//说明：顾客支付过钱了,商家可以发货了
                order.PayDateTime = DateTime.Now;

                new CourseOrdersBll().Update(order);
                //信守承诺，下订单的时候课程是什么价格就是什么价格，因为刚下了订单后可能课程就涨价了
                //因此不做价格检查

                //发货
                //给 "前台用户--课程" 表，添加新的记录
                UserCourses userCourse = new UserCourses();
                userCourse.CourseId = order.CourseId;//购买的课程id
                userCourse.ExpireDateTime = DateTime.Now.AddYears(1);//课程有效期一年
                userCourse.UserId = order.UserId;
                UserCoursesBll userCourseBll = new UserCoursesBll();
                userCourseBll.Add(userCourse);
                long courseId = order.CourseId;
                RPWRazor.RPRazorHelper.OutputRazor(context, "~/BuyCourse/BuyCourseOk.cshtml", courseId);
                #endregion

            }
            else
            {
                context.Response.Write("支付失败！");
            }

        }

    }

}