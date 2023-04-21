﻿using ISpan.InseparableCore.ViewModels;
using Microsoft.Extensions.Options;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class CinemaRepository
    {
        private readonly InseparableContext _db;
        public CinemaRepository(InseparableContext db)
        {
            _db = db;
        }
        public IEnumerable<TCinemas> QueryAll()
        {
            var data = _db.TCinemas.Select(t => t);
            return data;
        }
        public IEnumerable<TCinemas> GetByCity(string city)
        {
            var data = _db.TCinemas.Where(t => t.FCinemaRegion == city);
            return data;
        }
        public IEnumerable<TCinemas> GetByBrand(string brand)
        {
            var data = _db.TCinemas.Where(t => t.FCinemaName.Contains(brand));
            return data;
        }
    }
}