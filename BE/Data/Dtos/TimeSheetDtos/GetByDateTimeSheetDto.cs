namespace BE.Data.Dtos.TimeSheetDtos
{
    public class GetByDateTimeSheetDto
    {
        public int? idUser { get; set; }
        public string idProject { get; set; }
        public string? date { get; set; }

        public int? idUserCurrent { get; set; }
    }
}
