using BE.Data.Dtos.UserDtos;
using BE.Data.Models;

namespace BE.Data.Dtos.PostDtos
{
    public class PostCommentDto
    {
        public int id { get; set; }
        public string? content { get; set; }
        public int? postId { get; set; }
        public int? userCreated { get; set; }
        public DateTime? dateCreated { get; set; }
        public UserWithNameDto userCreatedNavigation { get; set; }
        public List<PostComments>? PostCommentsNavigations { get; set; }
        public int? parentId { get; set; }
        public UserWithNameDto? userComment { get; set; }
    }
}
