using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.Models.BLL.DTOs
{
    public class CinemaCreateDto
    {
        private CinemaEntity _cinema;
        public CinemaEntity cinemas
        {
            get { return _cinema; }
            set { _cinema = value; }
        }
        public CinemaCreateDto()
        {
            _cinema = new CinemaEntity();
        }
        public int FCinemaId { get => _cinema.FCinemaId; set => _cinema.FCinemaId = value; }
        public string FCinemaName { get => _cinema.FCinemaName; set => _cinema.FCinemaName = value; }
        public string FCinemaRegion { get => _cinema.FCinemaRegion; set => _cinema.FCinemaRegion = value; }

        public string FCinemaAddress { get => _cinema.FCinemaAddress; set => _cinema.FCinemaAddress = value; }

        public string FCinemaTel { get => _cinema.FCinemaTel; set => _cinema.FCinemaTel = value; }

        public double FLat { get => _cinema.FLat; set => _cinema.FLat = value; }

        public double FLng { get => _cinema.FLng; set => _cinema.FLng = value; }
        public string FTraffic { get => _cinema.FTraffic; set => _cinema.FTraffic = value; }
    }
}
