using RPW.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class LearningCardsBll
    {
        /// <summary>
        /// 检查学习卡密码是否对，如果对，则激活课程，返回值表示是否激活成功
        /// </summary>
        /// <param name="cardNum"></param>
        /// <param name="password"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CardAciveResult Active(string cardNum, string password, long userId)
        {
            //检查学习卡是否存在
            var card = dal.GetModelByCardNum(cardNum);
            if (card == null)
            {
                return CardAciveResult.CardNotFound;//学习卡不存在
            }
            //检查学习卡密码是否正确
            if (password != card.Password)
            {
                return CardAciveResult.ErrorPassWord;//密码错误
            }

            //学习卡正确，给用户激活课程
            //     1.用户的学习卡 状态码是false的才可以激活，是Ture的表示激活过了
            //     2.验证是不是false，这里使用了一个枚举
            if (!card.IsActive)//false
            {
                UserCoursesBll userCourseBll = new UserCoursesBll();
                UserCourses userCourse = new UserCourses();
                userCourse.CourseId = card.CourseId;
                userCourse.ExpireDateTime = DateTime.Now.AddDays(card.ExpireDays);
                userCourse.UserId = userId;
                userCourseBll.Add(userCourse);
                return CardAciveResult.OK;//激活成功
            }
            else//true
            {
                return CardAciveResult.CardHasActived;//表示用户激活过了
            }



        }

        /// <summary>
        /// 返回值表示是否生成成功，learnCards接收生成的学习卡
        /// </summary>
        /// <param name="courseId">为哪个课程号</param>
        /// <param name="cardNumPrefix">学习卡号前缀</param>
        /// <param name="expireDays">有效天数</param>
        /// <param name="startNo">学习卡开始数</param>
        /// <param name="endNo">学习卡结束数</param>
        /// <param name="learnCards">接收生成的学习卡</param>
        /// <returns></returns>
        public bool GenerateCards(long courseId, string cardNumPrefix, int expireDays, int startNo, int endNo, List<LearningCards> learnCards)
        {
            return dal.GenerateCards(courseId, cardNumPrefix, expireDays, startNo, endNo, learnCards);
        }

        //记录日志
        //private static ILog logger = LogManager.GetLogger(typeof(LearningCardsDal));
        /// <summary>
        /// 通过学习卡的卡号，获得一张学习卡
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        public LearningCards GetModelByCardNum(string cardNum)
        {
            return dal.GetModelByCardNum(cardNum);

        }
    }
    //声明一个枚举，学习卡激活后出现的情况
    public enum CardAciveResult { OK, CardNotFound, ErrorPassWord, CardHasActived };//添加未激活用户登录的情况
}
