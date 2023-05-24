namespace BE.Data.Models
{
    public class PostImages
    {
        public int id { get; set; }
        public string? imageName { get; set; }
        public string? imageType { get; set; }
        public string? pathImage { get; set; }
        public byte[] imageCode { get; set; }
        public DateTime? createdDate { get; set; }
        public int? postId { get; set; }
        public Posts? postImages { get; set; }
    }
}
