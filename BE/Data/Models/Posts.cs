using BE.Data.Models.Base;

namespace BE.Data.Models
{
    public class Posts: BaseEntity
    {
        public string? title { get; set; }
        public string? content { get; set; }
        public int categoryId { get; set; }

        public int userCreated { get; set; }
        public PostCategories postCate { get; set; }
        public ICollection<PostComments>? commentPostNavigations { get; set; }
        public ICollection<PostImages>? imagePostsNavigations { get; set; } = null;

    }
}
