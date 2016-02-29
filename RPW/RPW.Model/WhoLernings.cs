/**  版本信息模板在安装目录下，可自行修改。
* WhoLernings.cs
*
* 功 能： N/A
* 类 名： WhoLernings
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/9/13 17:42:58   N/A    初版
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
	/// WhoLernings:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class WhoLernings
	{
		public WhoLernings()
		{}
		#region Model
		private long _id;
		private string _name;
		private string _learningtime;
		private string _segmentname;
		private string _schoolname;
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
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string LearningTime
		{
			set{ _learningtime=value;}
			get{return _learningtime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string SegmentName
		{
			set{ _segmentname=value;}
			get{return _segmentname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string SchoolName
		{
			set{ _schoolname=value;}
			get{return _schoolname;}
		}
		#endregion Model

	}
}

