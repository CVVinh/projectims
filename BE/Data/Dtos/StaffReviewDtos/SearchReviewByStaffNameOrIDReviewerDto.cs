namespace BE.Data.Dtos.StaffReviewDtos
{
    public class SearchReviewByStaffNameOrIDReviewerDto
    {
        public int? id { get; set; }
        public int staffReviewTime { get; set; }
        public int companyBranhId { get; set; }
        //public string? name { get; set; }

        public int idUserCurrent { get; set; }
    }
}
