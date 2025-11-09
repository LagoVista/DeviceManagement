// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d00a2c3c8767da25a77f7359c602c9f2e488f399105a67079c3584398a250c36
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceLogRepo
    {
        Task AddLogEntryAsync(DeviceRepository deviceRepo, DeviceLog logEntry);

        Task<ListResponse<DeviceLog>> GetForDateRangeAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest);
    }
}
