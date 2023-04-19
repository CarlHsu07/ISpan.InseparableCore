using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.Models.BLL.Interfaces
{
    public interface ITicketOrderRepository
    {
        void Create(TicketOrderEntity ticket);
        TTicketOrderDetails GetBySeat(int? seesionid, bool status, int? seat);
    }
}
