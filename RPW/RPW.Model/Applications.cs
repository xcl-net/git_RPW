/**  版本信息模板在安装目录下，可自行修改。
* Applications.cs
*
* 功 能： N/A
* 类 名： Applications
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/9/11 1:16:38   N/A    初版
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
	/// Applications:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Applications
	{
		public Applications()
		{}
		#region Model
		private long _id;
		private string _name;
		private string _telephone;
		private string _qq;
		private string _school;
		private string _course;
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
		public string Telephone
		{
			set{ _telephone=value;}
			get{return _telephone;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string QQ
		{
			set{ _qq=value;}
			get{return _qq;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string School
		{
			set{ _school=value;}
			get{return _school;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Course
		{
			set{ _course=value;}
			get{return _course;}
		}
		#endregion Model

	}
}

