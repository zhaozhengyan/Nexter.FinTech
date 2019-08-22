using FinTech.Domain;

namespace FinTech.API.Wechat.Dto
{
    public class CategoryRequest
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public CategoryType Type { get; set; }
        public long Id { get; set; }
    }
}