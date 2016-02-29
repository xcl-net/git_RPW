using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPW.Model;

namespace RPW.Bll
{
    public partial class CourseOrdersBll
    {
        /// <summary>
        /// 添加一条订单记录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public long AddOrder(long userId, long courseId)
        {
            CourseOrders order = new CourseOrders();
            order.CourseId = courseId;
            order.CreateDateTime = DateTime.Now;
            order.IsPayed = false;
            order.UserId = userId;
            return Add(order);
        }
    }
}
