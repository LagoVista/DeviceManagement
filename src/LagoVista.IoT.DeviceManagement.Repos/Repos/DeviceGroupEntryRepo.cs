using LagoVista.IoT.DeviceManagement.Core.Repos;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using LagoVista.Core.Models;
using System.Threading.Tasks;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core;
using LagoVista.CloudStorage.Storage;
using LagoVista.IoT.Logging.Loggers;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceGroupEntryRepo : LagoVista.CloudStorage.Storage.TableStorageBase<DeviceGroupEntry>, IDeviceGroupEntryRepo
    {
        public DeviceGroupEntryRepo(IAdminLogger logger) : base(logger)
        {

        }


        public Task AddDeviceToGroupAsync(DeviceRepository deviceRepo, EntityHeader device, EntityHeader deviceGroup)
        {
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            var groupEntry = new DeviceGroupEntry()
            {
                RowKey = Guid.NewGuid().ToId(),
                PartitionKey = deviceGroup.Id,
                GroupId = deviceGroup.Id,
                GroupName = deviceGroup.Text,
                DeviceId = device.Id,
                DeviceName = device.Text
            };

            return InsertAsync(groupEntry);
        }

        public async Task<IEnumerable<EntityHeader>> GetDevicesInGroupAsync(DeviceRepository deviceRepo, string deviceGroupId)
        {
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            var devices = new List<EntityHeader>();

            var results = await base.GetByParitionIdAsync(deviceGroupId);
            foreach(var result in results)
            {
                devices.Add(new EntityHeader() { Id = result.DeviceId, Text = result.DeviceName });
            }

            return devices;
        }

        public async Task RemoveDeviceFromGroupAsync(DeviceRepository deviceRepo, string deviceGroupId, string deviceId)
        {
            SetConnection(deviceRepo.DeviceArchiveStorageSettings.AccountId, deviceRepo.DeviceArchiveStorageSettings.AccessKey);
            var result = await base.GetByFilterAsync(FilterOptions.Create("PartitionKey", FilterOptions.Operators.Equals, deviceGroupId), FilterOptions.Create("DeviceId", FilterOptions.Operators.Equals, deviceId));

            if (result.Any())
            {
                await base.RemoveAsync(result.First());
            }

        }
    }
}
