using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceStatusChangeRepo
    {
        Task AddDeviceStatusAsync(DeviceRepository deviceRepo, DeviceStatus status);
        Task<DeviceStatus> GetDeviceStatusAsync(DeviceRepository deviceRepo, string deviceUniqueId);
        Task UpdateDeviceStatusAsync(DeviceRepository deviceRepo, DeviceStatus status);
        Task<ListResponse<DeviceStatus>> GetDeviceStatusHistoryAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request);
        Task<ListResponse<DeviceStatus>> GetWatchdogDeviceStatusAsync(DeviceRepository deviceRepo, ListRequest request);
    }
}
