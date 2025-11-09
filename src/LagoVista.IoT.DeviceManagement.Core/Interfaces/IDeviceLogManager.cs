// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9b0c41b4a3643b15074890c2dac574bd0962f7e2e8cf2d780c87f8a242f3b48a
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceLogManager
    {
        Task AddEntryAsync(DeviceRepository deviceRepo, DeviceLog logEntry, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceLog>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user);
    }
}
