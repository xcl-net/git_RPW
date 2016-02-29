using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Model
{
    //将字段封装到model中，实现规范的三层功能；
    public class AdminOperationLogSearchOption
    {
        //是否通过用户名查询
        public bool SearchByUserName { get; set; }

        //是否通过日期查询
        public bool SearchByCreateDateTime { get; set; }

        //是否通过关键字查询
        public bool SearchByDesc { get; set; }

        //UI赋值用户名字段
        public string UserName { get; set; }

        //UI赋值到起始时间字段
        public DateTime StartTime { get; set; }

        //UI赋值到结束时间字段
        public DateTime EndTime { get; set; }

        //UI赋值到关键字字段
        public string Description { get; set; }
    }
}
