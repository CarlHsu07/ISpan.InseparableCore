using ISpan.InseparableCore.ViewModels;
using System;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace ISpan.InseparableCore.Models.DAL.Repo
{
	public class MemberRepository
	{
		private readonly InseparableContext _context;
		public MemberRepository(InseparableContext context)
		{
			_context = context;
		}

		public IEnumerable<MemberVM> members(string keyword)
		{
			var data = _context.TMembers.Where(t => t.FFirstName.Contains(keyword)
												|| t.FLastName.Contains(keyword)
												|| t.FAddress.Contains(keyword)
												|| t.FIntroduction.Contains(keyword)
												|| t.FArea.FAreaName.Contains(keyword)
												|| t.FEmail.Contains(keyword));
			
			List<MemberVM> list = new List<MemberVM>();
			foreach (var member in data)
			{
                MemberVM vm = new MemberVM();
                vm.members = member;
				list.Add(vm);
			}
			return list;
		}
		
	}
}

