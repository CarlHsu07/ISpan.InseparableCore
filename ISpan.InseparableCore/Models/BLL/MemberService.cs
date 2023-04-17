﻿using ISpan.InseparableCore.Models.DAL;
using Microsoft.EntityFrameworkCore;

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
    }
}