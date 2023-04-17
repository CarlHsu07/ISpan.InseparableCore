﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models.DAL
{
    public partial class TAdministrators
    {
        /// <summary>
        /// 管理員流水號
        /// </summary>
        public int FId { get; set; }
        /// <summary>
        /// 管理員ID
        /// </summary>
        public string FAdministratorId { get; set; }
        /// <summary>
        /// 管理員姓氏
        /// </summary>
        public string FLastName { get; set; }
        /// <summary>
        /// 管理員名字
        /// </summary>
        public string FFirstName { get; set; }
        /// <summary>
        /// 管理員電子郵件地址
        /// </summary>
        public string FEmail { get; set; }
        /// <summary>
        /// 管理員密碼加密值
        /// </summary>
        public string FPasswordHash { get; set; }
        /// <summary>
        /// 管理員密碼鹽值
        /// </summary>
        public string FPasswordSalt { get; set; }
        /// <summary>
        /// 管理員註冊時間
        /// </summary>
        public DateTime FSignUpTime { get; set; }
    }
}