namespace BE.Data.Dtos.CustomerDtos
{
    public class UpdateCustomerDto
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public byte? gender { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? company { get; set; }
        public string? address { get; set; }
        public byte workStatus { get; set; }
        public string? identityCard { get; set; }
        public string? accountNumber { get; set; }
        public int? userUpdated { get; set; }
        public string FullName
        {
            get { return $"{lastName} {firstName}"; }
        }
    }
}
