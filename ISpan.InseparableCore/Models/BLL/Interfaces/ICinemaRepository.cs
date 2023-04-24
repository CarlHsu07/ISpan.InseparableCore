using ISpan.InseparableCore.Models.BLL.Cores;
using ISpan.InseparableCore.Models.DAL;

namespace ISpan.InseparableCore.Models.BLL.Interfaces
{
    public interface ICinemaRepository
    {
        void Create(CinemaEntity entity);
        TCinemas GetByName(string fCinemaName);
    }
}
