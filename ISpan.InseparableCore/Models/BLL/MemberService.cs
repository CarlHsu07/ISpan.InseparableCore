﻿using ISpan.InseparableCore.Models.DAL;
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
        
        /// <summary>
        /// 判斷Email是否存在
        /// </summary>
        /// <param name="memberEmail"></param>
        /// <returns>如果Email存在就回傳true，否則回傳false</returns>
        public bool IsEmailExist(string memberEmail)
        {
            bool isExist = false;

            if (!string.IsNullOrEmpty(memberEmail))
            {
                isExist = _context.TMembers.Any(f => f.FEmail == memberEmail);
            }

            return isExist;
        }

        /// <summary>
        /// 產生驗證碼，用於會員信箱驗證
        /// </summary>
        /// <returns></returns>
        public string GenerateVerificationCode()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// 產生 會員ID 的方法
        /// </summary>
        /// <returns>回傳一個會員ID的字串，例如M2023050300001</returns>
        public string GenerateMemberId()
        {
            String todayDate = DateTime.Now.ToString("yyyyMMdd"); // 今天日期
            string newMemberID = string.Empty; // 新的會員ID
            string newSequence = string.Empty; // 新的五位數序號

            string? lastMemberID = _context.TMembers
                .OrderByDescending(m => m.FSignUpTime)
                .FirstOrDefault()?.FMemberId ?? null;

            if (string.IsNullOrEmpty(lastMemberID)) // 若DB中沒任何會員
            {
                newMemberID = CreateFirstMemberIDToday(todayDate); // 產生當日第一筆會員ID
            }
            else
            {
                // todo 有bug，序號不正確
                string dateString = lastMemberID.Substring(1, 8);

                if (dateString == todayDate) // 今天有會員註冊
                {
                    int length = lastMemberID.Length;
                    string lastFiveChars = length >= 5 ? lastMemberID.Substring(length - 5) : lastMemberID;
                    newSequence = (int.Parse(lastFiveChars) + 1).ToString().PadLeft(5, '0');
                    newMemberID = "M" + todayDate + newSequence; // 將日期和序號結合，形成 newMemberID，例如 M2023050300001
                }
                else
                {
                    newMemberID = CreateFirstMemberIDToday(todayDate);
                }
            }

            return newMemberID;
        }

        // 產生 今天第一個會員ID 的方法
        private static string CreateFirstMemberIDToday(string todayDate)
        {
            string newSequence = "1"; // 第一位會員，序號為1
            string sequenceString = newSequence.PadLeft(5, '0'); // 將序號轉為固定長度的字串，補'0'至5位數

            return "M" + todayDate + sequenceString; // 將日期和序號結合，形成 newMemberID，例如 M2023050300001
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

        // 取得一個好友紀錄
        public TFriends? GetOneFriendShip(int memberId, int friendId)
        {
            return _context.TFriends.FirstOrDefault(f =>
                    (f.FMemberId == memberId && f.FFriendId == friendId) ||
                    (f.FMemberId == friendId && f.FFriendId == memberId));
        }

        // 取得好友清單
        public async Task<List<CFriendListViewModel>> GetFriendListAsync(int? memberId)
        {
            List<CFriendListViewModel> friendList = new List<CFriendListViewModel>();

            if (memberId != null)
            {
                // 取得好友的member Fid
                var friends = await _context.TFriends
                    .Where(f => (f.FMemberId == memberId || f.FFriendId == memberId) && f.FFriendId != memberId)
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

        /// <summary>
        /// 驗證會員信箱是否存在
        /// </summary>
        /// <param name="member"></param>
        /// <param name="token"></param>
        /// <returns>ue</returns>
        public bool ConfirmEmail(TMembers member, string token)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(token))
            {
                // 驗證驗證碼是否相符
                if (member.FVerificationCode == token)
                {
                    member.FIsEmailVerified = true; // 驗證成功，更新會員Email驗證狀態
                    _context.Update(member);
                    _context.SaveChanges();

                    return true;
                }
            }

            return result;
        }
    }
}
