namespace BE.Data.Dtos.PostDtos
{
    public class UpdatePostCommentDto
    {
        public string? content { get; set; }
        public int? postId { get; set; }
        public int userUpdated { get; set; }
    }
}
