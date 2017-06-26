using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDeviceManager
    {
        Task<InvokeResult> AddDeviceAsync(Device device, EntityHeader user, EntityHeader org);

        Task<InvokeResult> UpdateDeviceAsync( Device device, EntityHeader org, EntityHeader user);

        Task<InvokeResult> DeleteDeviceAsync(string deviceRepositoryId, string id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(string repositoryId, string orgId,  int top, int take, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(string repositoryId, string locationId, int top, int take, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(string repositoryId, string status, int top, int take, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(string repositoryId, string configurationId, int top, int take, EntityHeader org, EntityHeader user);

        Task<Device> GetDeviceByDeviceIdAsync(string repositoryId, string id, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckIfDeviceIdInUse(string repositoryId, string id, EntityHeader org, EntityHeader user);

        Task<Device> GetDeviceByIdAsync(string repositoryId, string id, EntityHeader org, EntityHeader user);
    }
}
