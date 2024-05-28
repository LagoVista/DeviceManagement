using LagoVista.Core.Models;
using LagoVista.Core.Models.Geo;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using LagoVista.MediaServices.Models;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceManager
    {
        Task<InvokeResult> UpdateDeviceAsync(DeviceRepository deviceRepo, Device device, EntityHeader org, EntityHeader user);

        Task<InvokeResult> UpdateDeviceMacAddressAsync(DeviceRepository deviceRepo, string id, string macAddress, EntityHeader org, EntityHeader user);
        Task<InvokeResult> UpdateDeviceiOSBleAddressAsync(DeviceRepository deviceRepo, string id, string iosBLEAddress, EntityHeader org, EntityHeader user);

        Task<InvokeResult<Device>> GetDeviceByMacAddressAsync(DeviceRepository deviceRepo, string macAddress, EntityHeader org, EntityHeader user);
        Task<InvokeResult<Device>> GetDeviceByiOSBLEAddressAsync(DeviceRepository deviceRepo, string iosBLEAddress, EntityHeader org, EntityHeader user);

        /// <summary>
        /// Get the Device by the unique identifier that was generated for the device (not device id)
        /// </summary>
        /// <param name="deviceRepo">Repository connection data</param>
        /// <param name="id">Unique id for the device</param>
        /// <param name="org">User Org</param>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task<Device> GetDeviceByIdAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user, bool populateMetaData = false);

        /// <summary>
        /// Get the Device by the unique identifier that was generated for the device (not device id)
        /// </summary>
        /// <param name="deviceRepo">Repository connection data</param>
        /// <param name="id">Unique id for the device</param>
        /// <param name="org">User Org</param>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task<Device> GetDeviceByIdWithPinAsync(DeviceRepository deviceRepo, string id, string pin, EntityHeader org, EntityHeader user,  bool populateMetaData = false);


        /// <summary>
        /// Add a device note for a device
        /// </summary>
        /// <param name="deviceRepo"></param>
        /// <param name="id"></param>
        /// <param name="note"></param>
        /// <param name="org"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<InvokeResult> AddNoteAsync(DeviceRepository deviceRepo, string id, DeviceNote note, EntityHeader org, EntityHeader user);

        /// <summary>
        /// Get the Device by the unique identifier that was generated for the device (not device id)
        /// </summary>
        /// <param name="deviceRepo">Repository connection data</param>
        /// <param name="id">Unique id for the device</param>
        /// <param name="status">Status of the device</param>
        /// <param name="org">User Org</param>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task<InvokeResult> UpdateDeviceStatusAsync(DeviceRepository deviceRepo, string id, string status, EntityHeader org, EntityHeader user);

        /// <summary>
        /// Get the Device by the unique identifier that was generated for the device (not device id)
        /// </summary>
        /// <param name="deviceRepo">Repository connection data</param>
        /// <param name="id">Unique id for the device</param>
        /// <param name="customstatus">Custom defined status for the device</param>
        /// <param name="org">User Org</param>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task<InvokeResult> UpdateDeviceCustomStatusAsync(DeviceRepository deviceRepo, string id, string customstatus, EntityHeader org, EntityHeader user);

        /// <summary>
        /// Get the Device by the unique identifier that was generated for the device (not device id)
        /// </summary>
        /// <param name="deviceRepo">Repository connection data</param>
        /// <param name="id">Unique id for the device</param>
        /// <param name="geoLocation">Geo Location of device</param>
        /// <param name="org">User Org</param>
        /// <param name="user">User</param>
        /// <returns></returns>
        Task<InvokeResult> UpdateGeoLocationAsync(DeviceRepository deviceRepo, string id, GeoLocation geoLocation, EntityHeader org, EntityHeader user);


        /// <summary>
        /// Get the Device by the given Device ID that was entered by the user, this is the one that the device will send.
        /// </summary>
        /// <param name="deviceRepo">Repository connection data</param>
        /// <param name="deviceId">Assigned Device Id</param>
        /// <param name="org">User Org</param>
        /// <param name="user">User</param>
        /// /// <returns></returns>
        Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository deviceRepo, string deviceId, EntityHeader org, EntityHeader user, bool populateMetaData = false);

        Task<InvokeResult<Device>> AddDeviceAsync(DeviceRepository deviceRepo, Device device, bool reassign, EntityHeader org, EntityHeader user);
        
        /// <summary>
        /// Delete the device id by the generated device id.
        /// </summary>
        /// <param name="deviceRepo"></param>
        /// <param name="id"></param>
        /// <param name="org"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<InvokeResult> DeleteDeviceAsync(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesForDeviceRepoForUserAsync(DeviceRepository deviceRepo, string userId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<InvokeResult<MediaResource>> AddDeviceImageAsync(DeviceRepository deviceRepo, string deviceid, Stream strm, string fileName, string contentType, EntityHeader org, EntityHeader user);

        Task<MediaServices.Models.MediaItemResponse> GetDeviceImageAsync(DeviceRepository deviceRepo, string deviceId, string mediaId, EntityHeader org, EntityHeader user);


        Task<ListResponse<DeviceSummary>> GetDevicesForDeviceRepoAsync(DeviceRepository deviceRepo, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository deviceRepo, string locationId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusAsync(DeviceRepository deviceRepo, string customStatus, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string deviceConfigId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceSummary>> SearchByDeviceIdAsync(DeviceRepository deviceRepo, string searchString, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<ListResponse<Device>> GetFullDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<DependentObjectCheckResult> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<DeviceSummary>> GetChildDevicesAsync(DeviceRepository deviceRepo, string parentDeviceId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AttachChildDeviceAsync(DeviceRepository deviceRepo, string parentDeviceId, string childDeviceId, ListRequest listRequest, EntityHeader org, EntityHeader user);
        Task<InvokeResult> RemoveChildDeviceAsync(DeviceRepository deviceRepo, string parentDeviceId, string childDeviceId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AddDeviceExceptionAsync(DeviceRepository deviceRepo, DeviceException deviceException);

        Task<InvokeResult> ClearDeviceErrorAsync(DeviceRepository deviceRepo, string deviceId, string errorCode, EntityHeader org, EntityHeader user);

        Task<InvokeResult> ClearDeviceDataAsync(DeviceRepository deviceRepo, string deviceId, EntityHeader org, EntityHeader user);

        Task<ListResponse<DeviceConnectionEvent>> GetConnectionEventsForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest listRequest, EntityHeader org, EntityHeader user);

        Task<InvokeResult<Device>> CreateDeviceAsync(DeviceRepository deviceRepo, string deviceTypeId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<Device>> CreateDeviceForDeviceKeyAsync(DeviceRepository deviceRepo, string deviceTypeId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> AddDeviceToLocationAsync(DeviceRepository deviceRepo, string id, string locationId, EntityHeader org, EntityHeader user);

        Task<InvokeResult> RemoveDeviceFromLocation(DeviceRepository deviceRepo, string id, string locationId, EntityHeader org, EntityHeader user);

        Task<InvokeResult<string>> GenerateSecureDeviceLinkAsync(DeviceRepository deviceRepo, string id, string pin, EntityHeader org, EntityHeader user);
    }
}
