
using System;

namespace FinTech.Domain
{

    // Member
    public class Member
    {
        public Member() { }
        public Member(string nickName, string accountCode,string avatar)
        {
            GroupId = 0;
            NickName = nickName;
            Avatar = avatar;
            AccountCode = accountCode;
            CreatedAt = DateTime.Now;
        }

        public void SetGroup(long groupId)
        {
            GroupId = groupId;
        }

        public int GetRegDays()
        {
            return (int)DateTime.Now.Subtract(CreatedAt.Date).TotalDays;
        }

        public long Id { get; set; } // Id (Primary key)
        public long GroupId { get; set; } // GroupId

        ///<summary>
        /// 昵称
        ///</summary>
        public string NickName { get; set; } // NickName (length: 32)
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; } // NickName (length: 32)

        ///<summary>
        /// 微信Token
        ///</summary>
        public string AccountCode { get; set; } // AccountCode (length: 256)
        public DateTime CreatedAt { get; set; } // AccountCode (length: 256)
    }

}
