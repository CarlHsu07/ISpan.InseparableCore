using ISpan.InseparableCore.Models.BLL.Core;
using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.Models.BLL.Interface
{
    public interface ISessionRepository
    {   
        void Create(SessionEntity entity);
        void Edit(SessionEntity entity);

        TSessions GetByDateTime(int? room, DateTime date, TimeSpan time);
    }
}
