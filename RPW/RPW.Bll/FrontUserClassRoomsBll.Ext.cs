using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPW.Bll
{
    public partial class FrontUserClassRoomsBll
    {
        /// <summary>
        /// 得到一个对象实体,通过学生的id
        /// </summary>
        public RPW.Model.FrontUserClassRooms GetModelByStuId(long classId)
        {
            return dal.GetModelByStuId(classId);
        }

    }
}
