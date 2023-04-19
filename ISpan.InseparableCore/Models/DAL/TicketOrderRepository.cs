﻿using Microsoft.Data.Sqlite;

namespace ISpan.InseparableCore.Models.DAL
{
    public class TicketOrderRepository
    {
        private readonly InseparableContext _db;
        public TicketOrderRepository(InseparableContext db)
        {
            _db = db;
        }
        public void Create(TTicketOrderDetails ticket)
        {
            try
            {
                _db.TTicketOrderDetails.Add(ticket);
                _db.SaveChanges();
            }catch(SqliteException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<TTicketOrderDetails> GetSolid(int? seesionid,bool status)
        {
            var data = _db.TTicketOrderDetails.Where(t => t.FSessionId == seesionid && t.FStatus == status);
            return data;
        }
        public IEnumerable<TTicketOrderDetails> GetById(int? id)
        {
            var data = _db.TTicketOrderDetails.Where(t => t.FOrderId == id);
            return data;
        }
        public TTicketOrderDetails GetBySeat(int? seesionid, bool status,int? seat)
        {
            var data = _db.TTicketOrderDetails.Where(t => t.FSessionId == seesionid && t.FStatus == status).FirstOrDefault(t => t.FSeatId == seat);
            return data;
        }
    }
}
