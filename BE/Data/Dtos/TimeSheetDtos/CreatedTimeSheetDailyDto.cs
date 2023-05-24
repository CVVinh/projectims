namespace BE.Data.Dtos.TimeSheetDtos
{
    public class CreatedTimeSheetDailyDto
    {
        public int idUser { get; set; }
        public string idProject { get; set; }
        public string date { get; set; }
        public string taskContent { get; set; }
        public int layout { get; set; }
        public float spec { get; set; }
        public float api { get; set; }
        public float web { get; set; }
        public float utc { get; set; }
        public float ute { get; set; }
        public float integration { get; set; }
        public float deployment { get; set; }
        public float fixbug { get; set; }
        public float support { get; set; }
        public float others { get; set; }
        public float sum { get; set; }
        public int? userCreated { get; set; }
    }
}
