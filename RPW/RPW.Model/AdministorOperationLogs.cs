/**  版本信息模板在安装目录下，可自行修改。
* AdministorOperationLogs.cs
*
* 功 能： N/A
* 类 名： AdministorOperationLogs
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/8/7 14:19:51   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
namespace RPW.Model
{
	/// <summary>
	/// AdministorOperationLogs:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class AdministorOperationLogs
	{
		public AdministorOperationLogs()
		{}
		#region Model
		private long _id;
		private DateTime _operatedate;
		private long _userid;
		private string _description;
		/// <summary>
		/// 
		/// </summary>
		public long Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime OperateDate
		{
			set{ _operatedate=value;}
			get{return _operatedate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public long UserId
		{
			set{ _userid=value;}
			get{return _userid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Description
		{
			set{ _description=value;}
			get{return _description;}
		}
		#endregion Model

	}
}

