﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models.DAL
{
    public partial class TMovies
    {
        public TMovies()
        {
            TMovieActorDetails = new HashSet<TMovieActorDetails>();
            TMovieCategoryDetails = new HashSet<TMovieCategoryDetails>();
            TMovieCommentDetails = new HashSet<TMovieCommentDetails>();
            TMovieDirectorDetails = new HashSet<TMovieDirectorDetails>();
            TMovieKeywordDetails = new HashSet<TMovieKeywordDetails>();
            TMovieScoreDetails = new HashSet<TMovieScoreDetails>();
            TSessions = new HashSet<TSessions>();
            TTicketOrderDetails = new HashSet<TTicketOrderDetails>();
        }

        public int FMovieId { get; set; }
        public string FMovieName { get; set; }
        public string FMovieIntroduction { get; set; }
        public int FMovieLevelId { get; set; }
        public DateTime FMovieOnDate { get; set; }
        public DateTime? FMovieOffDate { get; set; }
        public int FMovieLength { get; set; }
        public string FMovieImagePath { get; set; }
        public decimal FMovieScore { get; set; }
        public string FMovieActors { get; set; }
        public string FMovieDirectors { get; set; }
        public bool FDeleted { get; set; }

        public virtual TMovieLevels FMovieLevel { get; set; }
        public virtual ICollection<TMovieActorDetails> TMovieActorDetails { get; set; }
        public virtual ICollection<TMovieCategoryDetails> TMovieCategoryDetails { get; set; }
        public virtual ICollection<TMovieCommentDetails> TMovieCommentDetails { get; set; }
        public virtual ICollection<TMovieDirectorDetails> TMovieDirectorDetails { get; set; }
        public virtual ICollection<TMovieKeywordDetails> TMovieKeywordDetails { get; set; }
        public virtual ICollection<TMovieScoreDetails> TMovieScoreDetails { get; set; }
        public virtual ICollection<TSessions> TSessions { get; set; }
        public virtual ICollection<TTicketOrderDetails> TTicketOrderDetails { get; set; }
    }
}