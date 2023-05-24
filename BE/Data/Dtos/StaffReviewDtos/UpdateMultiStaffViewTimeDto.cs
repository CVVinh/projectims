namespace BE.Data.Dtos.StaffReviewDtos
{
    public class UpdateMultiStaffViewTimeDto
    {
        public int? staffReviewTime { get; set; }
        public int? companyBranhId { get; set; }
        public int? reviewerId { get; set; }
        public string? detail { get; set; }
        public int? userUpdated { get; set; }
        public List<int> listStaffReviews { get; set; }
    }
}
