using BE.Data.Dtos.PostDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models.Base;

namespace BE.Data.Models
{
    public class PostComments : BaseEntity
    {
        public PostComments()
        {
            PostCommentsNavigations = new HashSet<PostComments>();
        }
        public string? content { get; set; }
        public int postId { get; set; }
        public Posts? postComment { get; set; }

        public int? parentId { get; set; }
        public PostComments? CommentsParent { get; set; }
        public ICollection<PostComments>? PostCommentsNavigations { get; set; }
        public int? idUserComment { get; set; }
        public Users? userComment { get; set; }
    }
}
