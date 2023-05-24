namespace BE.Data.Dtos.TimeSheetDtos
{
    public class ListTimeSheetTotalDto
    {
        public float layoutTotal { get; set; }
        public float specTotal { get; set; }
        public float apiTotal { get; set; }
        public float webTotal { get; set; }
        public float utcTotal { get; set; }
        public float uteTotal { get; set; }
        public float integrationTotal { get; set; }
        public float deploymentTotal { get; set; }
        public float fixbugTotal { get; set; }
        public float supportTotal { get; set; }
        public float othersTotal { get; set; }
        public float sumTotal { get; set; }
    }
}
