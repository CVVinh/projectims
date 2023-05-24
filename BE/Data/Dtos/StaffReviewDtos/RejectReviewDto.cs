namespace BE.Data.Dtos.StaffReviewDtos
{
    public class RejectReviewDto
    {
        public int idRejecters { get; set; }
        public string reason { get; set; }
        public int? userUpdated { get; set; }
    }
}
