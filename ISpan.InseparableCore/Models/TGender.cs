﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models
{
    public partial class TGender
    {
        public TGender()
        {
            TMembers = new HashSet<TMembers>();
        }

        public int FGenderId { get; set; }
        public string FGenderType { get; set; }

        public virtual ICollection<TMembers> TMembers { get; set; }
    }
}