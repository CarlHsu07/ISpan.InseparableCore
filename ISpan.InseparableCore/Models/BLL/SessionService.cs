using ISpan.InseparableCore.Models.BLL.Core;
using ISpan.InseparableCore.Models.BLL.Dtos;
using ISpan.InseparableCore.Models.BLL.Interface;

namespace ISpan.InseparableCore.Models.BLL
{
    public class SessionService
    {
        private readonly ISessionRepository repo;
        public SessionService(ISessionRepository _repo)
        {
            repo = _repo;
        }

        public void Create(SessionCreateDto dto)
        {

            var entityInDb = repo.GetByDateTime(dto.FRoomId, dto.FSessionDate, dto.FSessionTime);
            if (entityInDb != null) throw new Exception("該時段影廳已有電影播放!!");

            repo.Create(dto.session);
        }
        public void Edit(SessionEditDto dto)
        {

            var entityInDb = repo.GetByDateTime(dto.FRoomId, dto.FSessionDate, dto.FSessionTime);
            if (entityInDb != null && entityInDb.FSessionId != dto.FSessionId) throw new Exception("該時段影廳已有電影播放!!");

            repo.Edit(dto.session);
        }
    }
}
