using BE.Data.Models;

namespace BE.Data.Dtos.InfoDeviceDtos
{
    public class AppInDevice
    {
        public string name { get; set; }
        public string userName { get; set; }
        public Users user { get; set; }
        public InfoDevices DeviceInfo { get; set; }
        public List<DeviceInstalledApps> Applications { get; set; }
    }
}
