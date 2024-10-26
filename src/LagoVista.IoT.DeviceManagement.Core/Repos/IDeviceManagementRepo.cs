using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceManagementRepo
    {
        Task<string> Echo(string value);

        Task<InvokeResult> AddDeviceAsync(DeviceRepository repo, Device device);

        Task DeleteDeviceAsync(DeviceRepository repo, string id);

        Task UpdateDeviceAsync(DeviceRepository repo, Device device);

        Task DeleteDeviceByIdAsync(DeviceRepository repo, string deviceId);

        Task<ListResponse<DeviceSummary>> GetDevicesForRepositoryAsync(DeviceRepository repo, string orgId, ListRequest listRequest);

        Task<ListResponse<DeviceSummary>> GetDevicesForCustomerAsync(DeviceRepository repo, string orgId, string customerId, ListRequest listRequest);
        Task<ListResponse<DeviceSummary>> GetDevicesForRepositoryForUserAsync(DeviceRepository repo, string userId, string orgId, ListRequest listRequest);

        Task<ListResponse<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository repo, string locationId, ListRequest listRequest);

        Task<Device> GetDeviceByMacAddressAsync(DeviceRepository deviceRepo, string macAddress);
        Task<Device> GetDeviceByiOSBLEAddressAsync(DeviceRepository deviceRepo, string iosBLEAddress);

        Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository repo, string id);

        Task<bool> CheckIfDeviceIdInUse(DeviceRepository repo, string id, string orgid);

        Task<Device> GetDeviceByIdAsync(DeviceRepository repo, string id, bool throwOnRecordNotFound = true);

        Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository repo, string status, ListRequest listRequest);

        Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusAsync(DeviceRepository repo, string customStatus, ListRequest listRequest);
        Task<ListResponse<DeviceSummary>> GetChildDevicesAsync(DeviceRepository repo, string parentDeviceId, ListRequest listRequest);

        Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository repo, string configurationId, ListRequest listRequest);

        Task<ListResponse<Device>> GetFullDevicesWithConfigurationAsync(DeviceRepository repo, string configurationId, ListRequest listRequest);

        Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository repo, string deviceTypeId, ListRequest listRequest);
        Task<ListResponse<DeviceSummary>> SearchByDeviceIdAsync(DeviceRepository repo, string search, ListRequest listRequest);

        Task<ListResponse<DeviceSummaryData>> GetDeviceGroupSummaryDataAsync(DeviceRepository repo, string groupId, ListRequest listRequest);
    }
}