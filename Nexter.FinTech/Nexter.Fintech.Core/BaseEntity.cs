using Furion.DatabaseAccessor;
using Mapster;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTech.Domain
{
    public abstract class BaseEntity : IEntity
    {
        [Key]
        [AdaptIgnore(MemberSide.Destination)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    }
}
