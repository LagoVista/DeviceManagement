using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceGroupRepo
    {
        Task AddDeviceGroupAsync(DeviceGroup deviceGroup);

        Task UpdateDeviceGroupAsync(DeviceGroup deviceGroup);

        Task<DeviceGroup> GetDeviceGroupAsync(string id);

        Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(string orgId);
        
        Task DeleteDeviceGroupAsync(string deviceGroupId);

        Task<bool> QueryKeyInUseAsync(string key, string orgId);
    }
}
