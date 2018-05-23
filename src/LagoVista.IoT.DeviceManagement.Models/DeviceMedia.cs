namespace LagoVista.IoT.DeviceManagement.Models
{
    public class DeviceMedia
    {
        public string ItemId { get; set; }
        public string DeviceId { get; set; }
        public string TimeStamp { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }
    }
}
