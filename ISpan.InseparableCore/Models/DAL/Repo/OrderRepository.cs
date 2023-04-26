using ISpan.InseparableCore.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
    public class OrderRepository
    {
        private readonly InseparableContext _db;
        public OrderRepository(InseparableContext db)
        {
            _db = db;
        }
        public void Create(TOrders orders)
        {
            try
            {
                _db.TOrders.Add(orders);
                _db.SaveChanges();
            }
            catch (SqliteException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public TOrders GetByAll(TOrders orders)
        {
            if (orders == null)
                return null;

            var data = _db.TOrders.FirstOrDefault(t => t == orders);
            return data;
        }
        public TOrders GetById(int? id)
        {
            if (id == null)
                return null;

            var data = _db.TOrders.FirstOrDefault(t => t.FOrderId == id);
            return data;
        }
        public List<CorderVM> GetOrder(CorderSearch search)
        {

            var inseparableContext = _db.TOrders.Include(t => t.FCinema).Include(t => t.FMember).OrderByDescending(t=>t.FOrderId).Select(t=>t);
            if (inseparableContext == null)
                return null;

            if (search != null)
            {
                if (search.cinema != 0)
                    inseparableContext = inseparableContext.Where(t => t.FCinemaId == search.cinema);
                if (search.member != 0)
                    inseparableContext = inseparableContext.Where(t => t.FMemberId == search.member);
                if (search.max != null)
                    inseparableContext = inseparableContext.Where(t => t.FOrderDate <= search.max.AddDays(1));
                if (search.min != null)
                    inseparableContext = inseparableContext.Where(t => t.FOrderDate >= search.min);

            }
            List<CorderVM> data = new List<CorderVM>();
            foreach (var item in inseparableContext)
            {
                CorderVM vm = new CorderVM();
                vm.orders = item;
                vm.FCinema = item.FCinema;
                vm.FMember = item.FMember;

                data.Add(vm);
            }
            return data;
        }
        public TOrders GetOneOrder(int? id)
        {
            if (id == null)
                return null;

            var data = _db.TOrders
                .Include(t => t.FCinema)
                .Include(t => t.FMember)
                .FirstOrDefault(m => m.FOrderId == id);

            return data;
        }
        public void Delete(int? id)
        {
            if (id == null)
                throw new Exception("沒有資料可以刪除");

            var order = _db.TOrders.FirstOrDefault(t => t.FOrderId == id);
            if(order==null)
                throw new Exception("沒有資料可以刪除");

            order.FStatus = false;

            var ticket = _db.TTicketOrderDetails.Where(t => t.FOrderId == id);
            if (ticket== null)
                throw new Exception("沒有資料可以刪除");

            foreach (var item in ticket)
            {
                item.FStatus = false;
            }

            try
            {
                _db.SaveChanges();
            }
            catch (SqliteException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<CorderVM> GetMemberOrder(int? member,MemberOrderSearch search)
        {
            if (member == null)
                return null;

            var inseparableContext = _db.TOrders.Include(t => t.FCinema).Include(t => t.FMember).Where(t=>t.FMemberId==member).OrderByDescending(t => t.FOrderId).Select(t=>t);
            if (inseparableContext == null)
                return null;

            if (search != null)
            {
                if (search.max != null)
                    inseparableContext = inseparableContext.Where(t => t.FOrderDate <=search.max.AddDays(1));
                if (search.min != null)
                    inseparableContext = inseparableContext.Where(t => t.FOrderDate >= search.min);

            }
            List<CorderVM> data = new List<CorderVM>();
            foreach (var item in inseparableContext)
            {
                CorderVM vm = new CorderVM();
                vm.orders = item;
                vm.FCinema = item.FCinema;
                vm.FMember = item.FMember;

                data.Add(vm);
            }
            return data;
        }
    }
}
