using BE.Data.Models;

namespace BE.Data.Dtos.LeaveOffDtos
{
    public class UserInfoLeaveOffDtos
    {
        public int? CurrentUser { get; set; }
        public int? DayLeaveOff { get; set; } = 12 * 480;
        public int? NumberOfLeave { get; set; }
        public int? NumberOfLeaveAccept { get; set; }
        public int? NumberOfLeaveNoAccept { get; set; }
        public List<LeaveOff>? LeaveOffNavigations { get; set; }
        public List<LeaveOff>? AllLeaveOffNavigations { get; set; }
    }
}
