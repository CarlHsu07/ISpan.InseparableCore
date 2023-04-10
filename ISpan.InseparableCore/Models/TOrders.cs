﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ISpan.InseparableCore.Models
{
    public partial class TOrders
    {
        public TOrders()
        {
            TProductOrderDetails = new HashSet<TProductOrderDetails>();
            TTicketOrderDetails = new HashSet<TTicketOrderDetails>();
        }

        public int FOrderId { get; set; }
        public int FMemberId { get; set; }
        public int FCinemaId { get; set; }
        public DateTime FOrderDate { get; set; }
        public DateTime FModifiedTime { get; set; }
        public decimal FTotalMoney { get; set; }
        public string FStatus { get; set; }

        public virtual TCinemas FCinema { get; set; }
        public virtual TMembers FMember { get; set; }
        public virtual ICollection<TProductOrderDetails> TProductOrderDetails { get; set; }
        public virtual ICollection<TTicketOrderDetails> TTicketOrderDetails { get; set; }
    }
}