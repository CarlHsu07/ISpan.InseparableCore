using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.DAL;
using System.ComponentModel.DataAnnotations;

namespace ISpan.InseparableCore.ViewModels.TCinemasVM
{
    public class CTCinemasCreateVM
    {
        private CinemaCreateDto _cinema;
        public CinemaCreateDto cinemas
        {
            get { return _cinema; }
            set { _cinema = value; }
        }
        public CTCinemasCreateVM()
        {
            _cinema = new CinemaCreateDto();
        }
        public int FCinemaId { get => _cinema.FCinemaId; set => _cinema.FCinemaId = value; }
        [Display(Name = "名稱")]
        public string FCinemaName { get => _cinema.FCinemaName; set => _cinema.FCinemaName = value; }
        [Display(Name = "地區")]
        public string FCinemaRegion { get => _cinema.FCinemaRegion; set => _cinema.FCinemaRegion = value; }
        [Display(Name = "地址")]

        public string FCinemaAddress { get => _cinema.FCinemaAddress; set => _cinema.FCinemaAddress = value; }
        [Display(Name = "電話")]

        public string FCinemaTel { get => _cinema.FCinemaTel; set => _cinema.FCinemaTel = value; }
        [Display(Name = "座標(緯度)")]

        public double FLat { get => _cinema.FLat; set => _cinema.FLat = value; }
        [Display(Name = "座標(經度)")]

        public double FLng { get => _cinema.FLng; set => _cinema.FLng = value; }
        [Display(Name = "交通資訊")]

        public string FTraffic { get => _cinema.FTraffic; set => _cinema.FTraffic = value; }
    }
}
