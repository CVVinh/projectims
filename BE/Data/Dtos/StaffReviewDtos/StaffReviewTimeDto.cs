namespace BE.Data.Dtos.StaffReviewDtos
{
    public class StaffReviewTimeDto
    {
        public int? staffReviewTime { get; set; }
        public int? companyBranhId { get; set; }
        public int? reviewerId { get; set; }
        public int? staffReviewId { get; set; }
        public DateTime? dateReiew { get; set; }
        public string? detail { get; set; }
        public int? userCreated { get; set; }
        public bool isConfirm { get; set; }
    }
}
