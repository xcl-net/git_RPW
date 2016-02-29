using RPW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class UserCoursesBll
    {
        /// <summary>
        /// 查看当前用户的id，有哪些可以使用的课程(在有效期限的)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserCourses> GetUserCourses(long userId)
        {
            return dal.GetUserCourses(userId);
        }
        /// <summary>
        /// 检查当前用户，是否有这个课程
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckHasCourse(long courseId, long userId)
        {
            List<UserCourses> userCourses = GetUserCourses((long)userId);

            foreach (var course in userCourses)
            {
                if (course.CourseId == courseId)
                {
                    return true;
                }

            }
            return false;
        }

    }
}
