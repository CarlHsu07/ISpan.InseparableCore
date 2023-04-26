using ISpan.InseparableCore.Models.DAL;
using ISpan.InseparableCore.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace ISpan.InseparableCore.Models.BLL
{
    public class MemberService
    {
        private readonly InseparableContext _context;

        public MemberService(InseparableContext context)
        {
            _context = context;
        }

        // 產生 會員ID 的方法
        public string GenerateMemberId()
        {

            DateTime now = DateTime.Now; // 取得現在時間
            int newSequence = 0; // 新的序號

            // 查詢當日已經新增的會員數量
            int todayNewMemberCount = _context.TMembers.Count(m => m.FSignUpTime.Value.Date == now.Date);

            if (todayNewMemberCount == 0)
            {
                // 新的序號為當日新會員數量加1
                newSequence = todayNewMemberCount + 1;
            }
            else
            {
                string lastMemberSequence = _context.TMembers
                    .OrderByDescending(m => m.FSignUpTime.Value.Date == now.Date)
                    .FirstOrDefault().FMemberId;

                int length = lastMemberSequence.Length;
                string lastFiveChars = length >= 5 ? lastMemberSequence.Substring(length - 5) : lastMemberSequence;
                newSequence = int.Parse(lastFiveChars) + 1;
            }

            // 將序號轉換為固定長度的字串，補足至 5 位數，補足的字元為 0
            string sequenceString = newSequence.ToString().PadLeft(5, '0');

            // 將日期和序號結合，形成 FMemberId，格式為 yyyyMMdd-序號
            string fMemberId = "M" + now.ToString("yyyyMMdd") + sequenceString;

            return fMemberId;
        }

        //產生 會員註冊時間 的方法
        public DateTime GenerateSignUpTime()
        {
            return DateTime.Now;
        }

        // 判斷是否為好友
        public bool IsFriend(int? memberAId, int? memberBId)
        {
            bool isFriend = false;

            if (memberAId != null && memberBId != null)
            {
                isFriend = _context.TFriends.Any(f =>
                        (f.FMemberId == memberAId && f.FFriendId == memberBId) ||
                        (f.FMemberId == memberBId && f.FFriendId == memberAId));
            }

            return isFriend;
        }

        // 判斷是否為同一個會員
        public bool IsSameMember(int? memberAId, int? memberBId)
        {
            bool isCurrentMember = false;

            if (memberAId != null && memberBId != null)
            {
                isCurrentMember = memberAId == memberBId;
            }

            return isCurrentMember;
        }

        public async Task<List<CFriendListViewModel>> GetFriendListAsync(int? memberId)
        {
            List<CFriendListViewModel> friendList = new List<CFriendListViewModel>();

            if (memberId != null)
            {
                // 取得好友的member Fid
                var friends = await _context.TFriends
                    .Where(f => f.FMemberId == memberId)
                    .Select(f => f.FFriendId)
                    .ToListAsync();

                friendList = await _context.TMembers
                    .Where(m => friends.Contains(m.FId))
                    .Select(m => new CFriendListViewModel
                    {
                        Id = m.FId,
                        LastName = m.FLastName,
                        FirstName = m.FFirstName,
                        PhotoPath = m.FPhotoPath
                    })
                    .ToListAsync();
            }

            return friendList;
        }
    }
}
