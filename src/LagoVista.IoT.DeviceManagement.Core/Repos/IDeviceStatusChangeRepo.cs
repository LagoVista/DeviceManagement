// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 74052c78128192e0a5a841c0652f1ce8989c70cd792f8d2d08fe060269f515f1
// IndexVersion: 0
// --- END CODE INDEX META ---
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
