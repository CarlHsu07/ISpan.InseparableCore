﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models
{
    public partial class TKeywords
    {
        public TKeywords()
        {
            TArticleKeywordDetails = new HashSet<TArticleKeywordDetails>();
            TMovieKeywordDetails = new HashSet<TMovieKeywordDetails>();
        }

        public int FKeywordId { get; set; }
        public string FKeywordName { get; set; }

        public virtual ICollection<TArticleKeywordDetails> TArticleKeywordDetails { get; set; }
        public virtual ICollection<TMovieKeywordDetails> TMovieKeywordDetails { get; set; }
    }
}