﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models
{
    public partial class TCinemas
    {
        public TCinemas()
        {
            TOrders = new HashSet<TOrders>();
            TProducts = new HashSet<TProducts>();
            TRooms = new HashSet<TRooms>();
            TSessions = new HashSet<TSessions>();
        }

        public int FCinemaId { get; set; }
        public string FCinemaName { get; set; }
        public string FCinemaRegion { get; set; }
        public string FCinemaAddress { get; set; }
        public string FCinemaTel { get; set; }

        public virtual ICollection<TOrders> TOrders { get; set; }
        public virtual ICollection<TProducts> TProducts { get; set; }
        public virtual ICollection<TRooms> TRooms { get; set; }
        public virtual ICollection<TSessions> TSessions { get; set; }
    }
}