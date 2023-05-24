namespace BE.Data.Dtos.StaffReviewDtos
{
    public class GetAllMultiStaffViewTimeDto
    {
        public int id { get; set; }
        public int? staffReviewTime { get; set; }
        public int companyBranhId { get; set; }
        public string? branchName { get; set; }
        public int reviewerId { get; set; }
        public string? fullNameReviewer { get; set; }
        public int staffReviewId { get; set; }
        public string? fullNameStaffReview { get; set; }
        public string? fullNameStaffReviewList { get; set; } 
        public string? dateReview { get; set; }
        public string? detail { get; set; }

        public int? userCreated { get; set; }
        public string? fullNameUserCreated { get; set; }
        public bool isDeleted { get; set; }
        public bool? isAllReview { get; set; }
        public bool? isNotAllReview { get; set; }
        public bool isConfirm { get; set; }

    }
}
