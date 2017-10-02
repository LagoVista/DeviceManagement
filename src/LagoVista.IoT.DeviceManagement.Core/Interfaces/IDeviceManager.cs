using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceManagerRemote
    {
        Task<InvokeResult> UpdateDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user);
        
        /// <summary>
        /// Get the Device by the unique identifier that was generated for the device (not device id)
        /// </summary>
        /// <param name="deviceRepo">Repository connection data</param>
        /// <param name="id">Unique id for the device</param>
        /// <param name="org">User Org</param>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task<Device> GetDeviceByIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user);

        /// <summary>
        /// Get the Device by the given Device ID that was entered by the user, this is the one that the device will send.
        /// </summary>
        /// <param name="deviceRepo">Repository connection data</param>
        /// <param name="deviceId">Assigned Device Id</param>
        /// <param name="org">User Org</param>
        /// <param name="user">User</param>
        /// /// <returns></returns>
        Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository deviceRepo, string deviceId, EntityHeader org, EntityHeader user);
        IDeviceArchiveManager ArchiveManager { get; }
    }

    public interface IDeviceManager : IDeviceManagerRemote
    {
        Task<InvokeResult> AddDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user);
        
        /// <summary>
        /// Delete the device id by the generated device id.
        /// </summary>
        /// <param name="deviceRepo"></param>
        /// <param name="id"></param>
        /// <param name="org"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<InvokeResult> DeleteDeviceAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository deviceRepo, string orgId,  int top, int take, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository deviceRepo, string locationId, int top, int take, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, int top, int take, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, int top, int take, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user);
    }
}
