namespace BE.Data.Dtos.StaffReviewDtos
{
    public class CreateMultiStaffReviewTimeDto
    {
        public int staffReviewTime { get; set; }
        public int companyBranhId { get; set; }
        public int reviewerId { get; set; }
        public string? detail { get; set; }
        public DateTime dateReview { get; set; }
        public int userCreated { get; set; }
        public List<int> listStaffReviews { get; set; }
    }
}
