using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.BLL.Interfaces;

namespace ISpan.InseparableCore.Models.BLL
{
    public class CinemaService
    {
        private readonly ICinemaRepository repo;
        public CinemaService(ICinemaRepository _repo)
        {
            repo=_repo; 
        }

        public void Create(CinemaCreateDto dto)
        {
            var entityInDb = repo.GetByName(dto.FCinemaName);
            if (entityInDb != null) throw new Exception("該影院已存在!!");

            repo.Create(dto.cinemas);
        }
        public void Edit(CinemaCreateDto dto)
        {
            var entityInDb = repo.GetByName(dto.FCinemaName);
            if (entityInDb != null && entityInDb.FCinemaId!=dto.FCinemaId) throw new Exception("該影院已存在!!");

            repo.Edit(dto.cinemas);
        }
    }
}
