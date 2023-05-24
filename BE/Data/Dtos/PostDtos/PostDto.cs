using BE.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Dtos.PostDtos
{
    public class PostDto
    {
        public string? title { get; set; }
        public string? content { get; set; }
        public int categoryId { get; set; }
        public int userCreated { get; set; }
        [NotMapped]
        public ICollection<IFormFile>? imagePostsNavigationss { get; set; } = null;
    }
}
