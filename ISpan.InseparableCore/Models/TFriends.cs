﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models
{
    public partial class TFriends
    {
        /// <summary>
        /// 好友流水號
        /// </summary>
        public int FId { get; set; }
        /// <summary>
        /// 會員ID
        /// </summary>
        public int FMemberId { get; set; }
        /// <summary>
        /// 好友的序號
        /// </summary>
        public int FFriendNo { get; set; }
        /// <summary>
        /// 好友的ID
        /// </summary>
        public int FFriendId { get; set; }
        /// <summary>
        /// 成為好友的時間
        /// </summary>
        public DateTime FFriendDateTime { get; set; }

        public virtual TMembers FMember { get; set; }
    }
}