using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTech.Domain
{
    [Table("Groups")]
    public class Group : BaseEntity
    {
        public Group() { }
        public Group(string name, long createMemberId)
        {
            Name = name;
            CreateMemberId = createMemberId;
            CreatedAt = DateTime.Now;
        }
        public string Name { get; set; } // Name (length: 32)
        public long CreateMemberId { get; set; }
        public DateTime CreatedAt { get; set; } // CreatedAt
    }

}
