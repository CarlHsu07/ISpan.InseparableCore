﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models
{
    public partial class TMovieDirectorDetails
    {
        public int FSerialNumber { get; set; }
        public int FDirectorId { get; set; }
        public int FMovieId { get; set; }

        public virtual TDirectors FDirector { get; set; }
        public virtual TMovies FMovie { get; set; }
    }
}