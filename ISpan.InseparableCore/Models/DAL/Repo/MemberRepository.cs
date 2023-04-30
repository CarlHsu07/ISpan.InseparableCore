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

		public IEnumerable<TMembers> members(string keyword)
		{
			var data = _context.TMembers.Where(t => t.FFirstName.Contains(keyword)
												|| t.FLastName.Contains(keyword)
												|| t.FAddress.Contains(keyword)
										        || t.FIntroduction.Contains(keyword)
												|| t.FArea.FAreaName.Contains(keyword)
											    || t.FEmail.Contains(keyword));

			return data;
		}
	}
}

