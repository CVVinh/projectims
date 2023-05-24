using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Dtos.PostDtos
{
    public class UpdatePostDto
    {
        public string title { get; set; }
        public string content { get; set; }
        public int categoryId { get; set; }
        public int userUpdated { get; set; }
        [NotMapped]
        //public List<IFormFile>? imagePostsNavigationss { get; set; }
        public ICollection<IFormFile>? imagePostsNavigationss { get; set; } = null;
    }
}
