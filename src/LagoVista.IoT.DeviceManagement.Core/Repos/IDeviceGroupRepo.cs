using LagoVista.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceGroupRepo
    {
        Task AddDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup);

        Task UpdateDeviceGroupAsync(DeviceRepository deviceRepo, DeviceGroup deviceGroup);

        Task<DeviceGroup> GetDeviceGroupAsync(DeviceRepository deviceRepo, string id);

        Task<IEnumerable<DeviceGroupSummary>> GetDeviceGroupsForOrgAsync(DeviceRepository deviceRepo, string orgId);
        
        Task DeleteDeviceGroupAsync(DeviceRepository deviceRepo, string deviceGroupId);

        Task<bool> QueryKeyInUseAsync(DeviceRepository deviceRepo, string key, string orgId);
       
    }
}
