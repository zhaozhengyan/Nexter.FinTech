
using System;

namespace FinTech.Domain
{

    // Member
    public class Member
    {
        public long Id { get; set; } // Id (Primary key)
        public long GroupId { get; set; } // GroupId

        ///<summary>
        /// 昵称
        ///</summary>
        public string NickName { get; set; } // NickName (length: 32)

        ///<summary>
        /// 微信Token
        ///</summary>
        public string AccountCode { get; set; } // AccountCode (length: 256)
        public DateTime CreatedAt { get; set; } // AccountCode (length: 256)
    }

}
