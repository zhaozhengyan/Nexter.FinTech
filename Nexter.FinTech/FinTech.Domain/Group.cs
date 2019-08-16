using System;

namespace FinTech.Domain
{
    public class Group
    {
        public Group(string name, long createMemberId)
        {
            Name = name;
            CreateMemberId = createMemberId;
            CreatedAt = DateTime.Now;
        }

        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 32)
        public long CreateMemberId{ get; set; }
        public System.DateTime CreatedAt { get; set; } // CreatedAt
    }

}
