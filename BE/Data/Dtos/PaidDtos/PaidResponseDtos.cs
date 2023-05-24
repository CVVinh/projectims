using BE.Data.Models;

namespace BE.Data.Dtos.PaidDtos
{
    public class PaidResponseDtos
    {
        public int id { get; set; }
        public int paidPerson { get; set; }
        public string paidPersonName { get; set; }


        public int? personConfirm { get; set; }
        public string? personConfirmName { get; set; }

        public string? paidDate { get; set; }
        public string? createDate { get; set; }

        public int? projectId { get; set; }
        public string? nameProject { get; set; }
        public bool isDelPro { get; set; } = false;

        public int customerName { get; set; }
        public string customerFullName { get; set; }

        public decimal amountPaid { get; set; }
        public string amountPaidName { get; set; }

        public int paidReason { get; set; }
        public string paidReasonName { get; set; }

        public string? contentReason { get; set; }
        public string? reasonRefusal { get; set; }
        public int isAccept { get; set; } = 0;

        public ICollection<PaidImage>? paidImages { get; set; }
    }
}
