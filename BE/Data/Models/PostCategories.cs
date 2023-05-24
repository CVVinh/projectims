namespace BE.Data.Models
{
    public class PostCategories
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public ICollection<Posts>? postNavigations { get; set; }
    }
}
