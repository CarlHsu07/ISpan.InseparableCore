﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models.DAL
{
    public partial class TArticleKeywordDetails
    {
        public int FSerialNumber { get; set; }
        public int FArticleId { get; set; }
        public int FKeywordId { get; set; }

        public virtual TArticles FArticle { get; set; }
        public virtual TKeywords FKeyword { get; set; }
    }
}