namespace BE.Data.Dtos.PostDtos
{
    public class CreatePostCommentDto
    {
        public int id { get; set; }
        public string? content { get; set; }
        public int? postId { get; set; }
        public int? userCreated { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? parentId { get; set; }
        public int? idUserComment { get; set; }
    }
}
