﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models.DAL
{
    public partial class TMembers
    {
        public TMembers()
        {
            TActivityParticipants = new HashSet<TActivityParticipants>();
            TArticles = new HashSet<TArticles>();
            TComments = new HashSet<TComments>();
            TFriends = new HashSet<TFriends>();
            TMemberFavoriteMovieCategories = new HashSet<TMemberFavoriteMovieCategories>();
            TMemberPoints = new HashSet<TMemberPoints>();
            TMemberReportsFReportMember = new HashSet<TMemberReports>();
            TMemberReportsFReportedMember = new HashSet<TMemberReports>();
            TMovieScoreDetails = new HashSet<TMovieScoreDetails>();
            TOrders = new HashSet<TOrders>();
        }

        /// <summary>
        /// 會員流水號
        /// </summary>
        public int FId { get; set; }
        /// <summary>
        /// 會員ID
        /// </summary>
        public string FMemberId { get; set; }
        /// <summary>
        /// 姓氏
        /// </summary>
        public string FLastName { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string FFirstName { get; set; }
        /// <summary>
        /// 電子郵件的地址
        /// </summary>
        public string FEmail { get; set; }
        /// <summary>
        /// 密碼加密值
        /// </summary>
        public string FPasswordHash { get; set; }
        /// <summary>
        /// 密碼鹽值
        /// </summary>
        public string FPasswordSalt { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? FDateOfBirth { get; set; }
        /// <summary>
        /// 性別ID
        /// </summary>
        public int? FGenderId { get; set; }
        /// <summary>
        /// 手機號碼
        /// </summary>
        public string FCellphone { get; set; }
        /// <summary>
        /// 住址
        /// </summary>
        public string FAddress { get; set; }
        /// <summary>
        /// 區域ID
        /// </summary>
        public int? FAreaId { get; set; }
        /// <summary>
        /// 大頭貼的檔案路徑
        /// </summary>
        public string FPhotoPath { get; set; }
        /// <summary>
        /// 個人簡介
        /// </summary>
        public string FIntroduction { get; set; }
        /// <summary>
        /// 會員帳戶狀態
        /// </summary>
        public int? FAccountStatus { get; set; }
        /// <summary>
        /// 目前點數餘額
        /// </summary>
        public int? FTotalMemberPoint { get; set; }
        /// <summary>
        /// 會員註冊時間
        /// </summary>
        public DateTime? FSignUpTime { get; set; }

        public virtual TAccountStatuses FAccountStatusNavigation { get; set; }
        public virtual TAreas FArea { get; set; }
        public virtual TGenders FGender { get; set; }
        public virtual ICollection<TActivityParticipants> TActivityParticipants { get; set; }
        public virtual ICollection<TArticles> TArticles { get; set; }
        public virtual ICollection<TComments> TComments { get; set; }
        public virtual ICollection<TFriends> TFriends { get; set; }
        public virtual ICollection<TMemberFavoriteMovieCategories> TMemberFavoriteMovieCategories { get; set; }
        public virtual ICollection<TMemberPoints> TMemberPoints { get; set; }
        public virtual ICollection<TMemberReports> TMemberReportsFReportMember { get; set; }
        public virtual ICollection<TMemberReports> TMemberReportsFReportedMember { get; set; }
        public virtual ICollection<TMovieScoreDetails> TMovieScoreDetails { get; set; }
        public virtual ICollection<TOrders> TOrders { get; set; }
    }
}