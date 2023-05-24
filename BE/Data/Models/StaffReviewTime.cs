using BE.Data.Models.Base;

namespace BE.Data.Models
{
    public class StaffReviewTime: BaseEntity
    {
        public int staffReviewTime { get; set; }
        public int companyBranhId { get; set; }
        public int reviewerId { get; set; }
        public int staffReviewId { get; set; } 
        public DateTime dateReview { get; set; }
        public string? detail { get; set; }
        public bool isConfirm { get; set; } 
        public ICollection<StaffReview>? staffReviewNavigations { get; set; }
    }
}
