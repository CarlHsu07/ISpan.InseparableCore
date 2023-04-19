using ISpan.InseparableCore.Models.BLL.DTOs;
using ISpan.InseparableCore.Models.BLL.Interfaces;

namespace ISpan.InseparableCore.Models.BLL
{
    public class TicketOrderService
    {
        public readonly ITicketOrderRepository repo;

        public TicketOrderService(ITicketOrderRepository repo)
        {
            this.repo = repo;
        }

        public void Create(TicketOrderCreateDto dto)
        {

            var entityInDb = repo.GetBySeat(dto.FSessionId, true, dto.FSeatId);
            if (entityInDb != null) throw new Exception("該場次此座位已被選走!!");

            repo.Create(dto.ticket);
        }
    }
}
