namespace DMDService.Services.Models
{
    public class TargetDevice
    {
        public string StationId { get; set; }
        public string AreaId { get; set; }
        public string DeviceId { get; set; }

        public string ToTargetString()
        {
            return $"{StationId}_{AreaId}_{DeviceId}";
        }
    }
}
