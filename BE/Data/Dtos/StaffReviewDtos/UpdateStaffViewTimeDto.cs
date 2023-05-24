namespace BE.Data.Dtos.StaffReviewDtos
{
    public class UpdateStaffViewTimeDto
    {
        public int? staffReviewTime { get; set; }
        public int? companyBranhId { get; set; }
        public int? reviewerId { get; set; }
        public int? staffReviewId { get; set; }
        public DateTime? dateReiew { get; set; }
        public string? detail { get; set; }
        public int? userUpdated { get; set; }
    }
}
