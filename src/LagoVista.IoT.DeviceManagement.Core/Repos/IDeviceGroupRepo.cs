// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cadeabec8413455dc0c844c1b86a25bd998ede80b9cc976c9eeb69ead57d6ce3
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceGroupRepo
    {
        Task AddDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup);

        Task UpdateDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup);

        Task<DeviceGroup> GetDeviceGroupAsync(DeviceRepository deviceRepo, string id);

        Task<DeviceGroup> GetDeviceGroupByKeyAsync(DeviceRepository deviceRepo, string groupKey);

        Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(DeviceRepository deviceRepo, string orgId);
        
        Task DeleteDeviceGroupAsync(DeviceRepository deviceRepo, string deviceGroupId);

        Task<bool> QueryKeyInUseAsync(DeviceRepository deviceRepo, string key, string orgId);
        Task RemoveDeviceGromGroupAsync(DeviceRepository deviceRepo, string groupId, string id);
    }
}
