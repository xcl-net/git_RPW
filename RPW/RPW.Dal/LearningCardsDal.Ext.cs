using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPW.Model;
using Maticsoft.DBUtility;
using System.Data.SqlClient;
using System.Data;


namespace RPW.Dal
{
    public partial class LearningCardsDal
    {
        //记录日志
        //private static ILog logger = LogManager.GetLogger(typeof(LearningCardsDal));
        /// <summary>
        /// 通过学习卡的卡号，获得一张学习卡
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        public LearningCards GetModelByCardNum(string cardNum)
        {
            DataSet ds = DbHelperSQL.Query("select * from T_LearningCards where CardNum=@CardNum", new SqlParameter("@CardNum", cardNum));
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count <= 0)
            {
                //没有找到学习卡
                return null;
            }
            if (dt.Rows.Count > 1)//多加几道防护，虽然麻烦，但是更安全
            {
                //报异常信息
                throw new Exception("查到多张学习卡");
            }
            DataRow row = dt.Rows[0];//把这条记录存到model中
            return this.DataRowToModel(row);
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
            //如果生成结尾数，小于生成卡号的起始数值
            if (endNo < startNo)
            {
                throw new Exception("结束号码不能小于起始号码");
            }
            //
            //通过数据库中的事务，来保证生成的卡号冲突时候，保证回滚
            using (SqlConnection conn = new SqlConnection(DbHelperSQL.connectionString))
            {
                conn.Open();//1.打开连接
                using (SqlTransaction tran = conn.BeginTransaction())//2.开始数据库的事务
                using (SqlCommand cmd = conn.CreateCommand())//3.创建命令对象
                {
                    try
                    {
                        //4.使用sqlserver事务的时候要注意,添加这个，其他的数据库，不需要添加
                        cmd.Transaction = tran;
                        cmd.CommandText = "insert into T_LearningCards(CourseId,CardNum,ExpireDays,Password,ActiveDateTime,IsActive) output inserted.Id values(@CourseId,@CardNum,@ExpireDays,@Password,@ActiveDateTime,@IsActive)";
                        //声明的random对象，放到for循环的外部，这样，就降低生成相同密码的概率
                        Random random = new Random();
                        for (int i = startNo; i <= endNo; i++)
                        {
                            //1.每一个循环，都生成一张学习卡

                            string cardNum = cardNumPrefix + i;
                            string password = GeneratePassword(random);//写一个方法，随机生成密码
                            cmd.Parameters.Clear();//移除参数
                            cmd.Parameters.AddWithValue("@CourseId", courseId);
                            cmd.Parameters.AddWithValue("@CardNum", cardNum);
                            cmd.Parameters.AddWithValue("@ExpireDays", expireDays);
                            cmd.Parameters.AddWithValue("@Password", password);
                            cmd.Parameters.AddWithValue("@ActiveDateTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@IsActive", false);  //默认生成的学习卡是没有激活过的
                            long id = (long)cmd.ExecuteScalar();//得到新增主键的值

                            LearningCards card = new LearningCards();
                            card.Id = id;
                            card.CardNum = cardNum;
                            card.CourseId = courseId;
                            card.ExpireDays = expireDays;
                            card.Password = password;
                            //执行插入
                            learnCards.Add(card);//用泛型接收，生成成功的学习卡
                        }
                        tran.Commit();//提交事务
                        return true;

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                    }
                }
            }
            return false;
        }


        //生成随机密码的方法
        private static string GeneratePassword(Random rand)
        {
            StringBuilder sb = new StringBuilder();
            //尽量把1和0的排除在外,因为这些不太容易识别
            char[] chars = { '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'k', 'm' };
            for (int i = 0; i < 9; i++)
            {
                int index = rand.Next(0, chars.Length);//随机取一个数字
                char ch = chars[index];
                sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}
