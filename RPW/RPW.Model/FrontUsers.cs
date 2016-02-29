/**  版本信息模板在安装目录下，可自行修改。
* FrontUsers.cs
*
* 功 能： N/A
* 类 名： FrontUsers
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2015/9/13 23:21:54   N/A    初版
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
	/// FrontUsers:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class FrontUsers
	{
		public FrontUsers()
		{}
		#region Model
		private long _id;
		private string _name;
		private string _pwd;
		private string _email;
		private DateTime _regdatetime;
		private bool _isactive;
		private string _telephone;
		private string _qq;
		private string _schoolname;
		private string _specialty;
		private int? _graduate;
		private int? _enroalyear;
		private string _realname;
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
		public string Pwd
		{
			set{ _pwd=value;}
			get{return _pwd;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Email
		{
			set{ _email=value;}
			get{return _email;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime RegDateTime
		{
			set{ _regdatetime=value;}
			get{return _regdatetime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsActive
		{
			set{ _isactive=value;}
			get{return _isactive;}
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
		public string SchoolName
		{
			set{ _schoolname=value;}
			get{return _schoolname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Specialty
		{
			set{ _specialty=value;}
			get{return _specialty;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? Graduate
		{
			set{ _graduate=value;}
			get{return _graduate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? EnroalYear
		{
			set{ _enroalyear=value;}
			get{return _enroalyear;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string RealName
		{
			set{ _realname=value;}
			get{return _realname;}
		}
		#endregion Model

	}
}

