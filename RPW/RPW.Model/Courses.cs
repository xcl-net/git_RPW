/**  版本信息模板在安装目录下，可自行修改。
* Courses.cs
*
* 功 能： N/A
* 类 名： Courses
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/9/27 0:20:27   N/A    初版
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
	/// Courses:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Courses
	{
		public Courses()
		{}
		#region Model
		private long _id;
		private string _cname;
		private DateTime _createdatetime;
		private int _serialno;
		private string _descriptioncourses;
		private int? _price;
		/// <summary>
		/// 
		/// </summary>
		public long Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 课程名称
		/// </summary>
		public string CName
		{
			set{ _cname=value;}
			get{return _cname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateDateTime
		{
			set{ _createdatetime=value;}
			get{return _createdatetime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int SerialNo
		{
			set{ _serialno=value;}
			get{return _serialno;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string DescriptionCourses
		{
			set{ _descriptioncourses=value;}
			get{return _descriptioncourses;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Price
		{
			set{ _price=value;}
			get{return _price;}
		}
		#endregion Model

	}
}

