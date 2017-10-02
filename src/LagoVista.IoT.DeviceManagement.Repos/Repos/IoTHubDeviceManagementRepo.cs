using LagoVista.IoT.DeviceManagement.Core.Repos;
using System;
using System.Collections.Generic;
using System.Text;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LagoVista.Core.Validation;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class IoTHubDeviceManagementRepo : IDeviceManagementRepo
    {

        public IoTHubDeviceManagementRepo()
        {

        }

        public Task<InvokeResult> AddDeviceAsync(DeviceRepository repo, Device device)
        {
            var deviceIdRegEx = new Regex(@"^[A-Za-z0-9\-:.+%_#*?!(),=@;$']{1,128}$");

            var apiVersion = "2016-11-14";

            var uri = $"https://{repo.Key}.bytemaster.azure-devices.net/devices/{device.DeviceId}?api-version={apiVersion}";

            return Task.FromResult(InvokeResult.Success);
        }

        public Task<bool> CheckIfDeviceIdInUse(DeviceRepository repo, string id, string orgid)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDeviceAsync(DeviceRepository repo, string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDeviceByIdAsync(DeviceRepository repo, string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository repo, string id)
        {
            throw new NotImplementedException();
        }

        public Task<Device> GetDeviceByIdAsync(DeviceRepository repo, string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository repo, string locationId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository repo, string orgId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository repo, string status, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository repo, string configurationId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository repo, string deviceTypeId, int top, int take)
        {
            throw new NotImplementedException();
        }

        public Task UpdateDeviceAsync(DeviceRepository repo, Device device)
        {
            throw new NotImplementedException();
        }
    }
}
