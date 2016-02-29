/**  版本信息模板在安装目录下，可自行修改。
* Segments.cs
*
* 功 能： N/A
* 类 名： Segments
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/8/15 20:15:59   N/A    初版
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
	/// Segments:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Segments
	{
		public Segments()
		{}
		#region Model
		private long _id;
		private string _name;
		private int _serialno;
		private string _videocode;
		private string _note;
		private long _chapterid;
		private DateTime _createdatetime;
		private string _descriptionsegments;
		/// <summary>
		/// 
		/// </summary>
		public long Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 段落名称
		/// </summary>
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 为段落分配的逻辑序列号
		/// </summary>
		public int SerialNo
		{
			set{ _serialno=value;}
			get{return _serialno;}
		}
		/// <summary>
		/// 视频代码片段
		/// </summary>
		public string VideoCode
		{
			set{ _videocode=value;}
			get{return _videocode;}
		}
		/// <summary>
		/// 课程笔记
		/// </summary>
		public string Note
		{
			set{ _note=value;}
			get{return _note;}
		}
		/// <summary>
		/// 属于哪个章节(与章节的逻辑序列号是外键的关系)
		/// </summary>
		public long ChapterId
		{
			set{ _chapterid=value;}
			get{return _chapterid;}
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
		public string DescriptionSegments
		{
			set{ _descriptionsegments=value;}
			get{return _descriptionsegments;}
		}
		#endregion Model

	}
}

