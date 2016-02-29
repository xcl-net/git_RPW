/**  版本信息模板在安装目录下，可自行修改。
* Chapters.cs
*
* 功 能： N/A
* 类 名： Chapters
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/8/15 20:15:58   N/A    初版
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
	/// Chapters:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Chapters
	{
		public Chapters()
		{}
		#region Model
		private long _id;
		private string _name;
		private int _serialno;
		private long _courseid;
		private DateTime _createdatetime;
		private string _descriptionchapter;
		/// <summary>
		/// 
		/// </summary>
		public long Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 章节名称
		/// </summary>
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 为章节分配的逻辑序列号
		/// </summary>
		public int SerialNo
		{
			set{ _serialno=value;}
			get{return _serialno;}
		}
		/// <summary>
		/// 章节属于哪个课程(与课程表中的id建立外键的关系)
		/// </summary>
		public long CourseId
		{
			set{ _courseid=value;}
			get{return _courseid;}
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
		public string DescriptionChapter
		{
			set{ _descriptionchapter=value;}
			get{return _descriptionchapter;}
		}
		#endregion Model

	}
}

