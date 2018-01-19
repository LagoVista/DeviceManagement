using LagoVista.IoT.DeviceManagement.Core.Repos;
using System.Linq;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.PlatformSupport;
using LagoVista.IoT.Logging.Loggers;
using System;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Resources;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceManagementRepo : DocumentDBRepoBase<Device>, IDeviceManagementRepo
    {
        private const string AZURE_DEVICE_CLIENT_STR = "HostName={0}.azure-devices.net;SharedAccessKeyName={1};SharedAccessKey={2}";

        private bool _shouldConsolidateCollections;

        public DeviceManagementRepo(IDeviceManagementSettings repoSettings, IAdminLogger logger) : base(logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
        }

        public DeviceManagementRepo(IAdminLogger logger) : base(logger)
        {
            _shouldConsolidateCollections = true;
        }

        protected override String GetCollectionName()
        {
            return "Devices";
        }

        public async Task<InvokeResult> AddDeviceAsync(DeviceRepository deviceRepo, Device device)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var iotHubDevice = new Microsoft.Azure.Devices.Device(device.DeviceId)
                {
                    Authentication = new Microsoft.Azure.Devices.AuthenticationMechanism()
                    {
                        Type = Microsoft.Azure.Devices.AuthenticationType.Sas,
                        SymmetricKey = new Microsoft.Azure.Devices.SymmetricKey()
                        {
                            PrimaryKey = device.PrimaryAccessKey,
                            SecondaryKey = device.SecondaryAccessKey,
                        }
                    }
                };

                var connString = String.Format(AZURE_DEVICE_CLIENT_STR, deviceRepo.ResourceName, deviceRepo.AccessKeyName, deviceRepo.AccessKey);
                var regManager = Microsoft.Azure.Devices.RegistryManager.CreateFromConnectionString(connString);
                var existingDevice = await regManager.GetDeviceAsync(device.DeviceId);
                if (existingDevice != null)
                {
                    return InvokeResult.FromErrors(ErrorCodes.DeviceExistsInIoTHub.ToErrorMessage($"DeviceID={device.DeviceId}"));
                }
                await regManager.AddDeviceAsync(iotHubDevice);
            }
            await CreateDocumentAsync(device);

            return InvokeResult.Success;
        }

        public async Task DeleteDeviceAsync(DeviceRepository deviceRepo, string id)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id);

            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            await DeleteDocumentAsync(id);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var connString = String.Format(AZURE_DEVICE_CLIENT_STR, deviceRepo.ResourceName, deviceRepo.AccessKeyName, deviceRepo.AccessKey);
                var regManager = Microsoft.Azure.Devices.RegistryManager.CreateFromConnectionString(connString);
                await regManager.RemoveDeviceAsync(device.DeviceId);
            }
        }

        public async Task DeleteDeviceByIdAsync(DeviceRepository deviceRepo, string deviceId)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var device = await this.GetDeviceByDeviceIdAsync(deviceRepo, deviceId);
            await DeleteDeviceAsync(deviceRepo, device.Id);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var connString = String.Format(AZURE_DEVICE_CLIENT_STR, deviceRepo.ResourceName, deviceRepo.AccessKeyName, deviceRepo.AccessKey);
                var regManager = Microsoft.Azure.Devices.RegistryManager.CreateFromConnectionString(connString);
                await regManager.RemoveDeviceAsync(device.DeviceId);
            }
        }

        public async Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository deviceRepo, string id)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            return (await base.QueryAsync(device => device.DeviceId == id && device.DeviceRepository.Id == deviceRepo.Id)).FirstOrDefault();
        }

        public async Task<bool> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string id, string orgid)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            return (await base.QueryAsync(device => device.OwnerOrganization.Id == id && device.DeviceRepository.Id == deviceRepo.Id && device.DeviceId == id)).Any();
        }

        public Task<Device> GetDeviceByIdAsync(DeviceRepository deviceRepo, string id)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            return GetDocumentAsync(id);
        }

        public async Task UpdateDeviceAsync(DeviceRepository deviceRepo, Device device)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            await UpsertDocumentAsync(device);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var connString = String.Format(AZURE_DEVICE_CLIENT_STR, deviceRepo.ResourceName, deviceRepo.AccessKeyName, deviceRepo.AccessKey);
                var regManager = Microsoft.Azure.Devices.RegistryManager.CreateFromConnectionString(connString);
                var iotHubDevice = await regManager.GetDeviceAsync(device.DeviceId);
                if (iotHubDevice.Authentication.Type != Microsoft.Azure.Devices.AuthenticationType.Sas)
                {
                    throw new InvalidOperationException("Currently only support Shared Access Key Authentication");
                }

                iotHubDevice.Authentication.SymmetricKey = new Microsoft.Azure.Devices.SymmetricKey()
                {
                    PrimaryKey = device.PrimaryAccessKey,
                    SecondaryKey = device.SecondaryAccessKey,
                };

                await regManager.AddDeviceAsync(iotHubDevice);
            }
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForLocationIdAsync(DeviceRepository deviceRepo, string locationId, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.Location.Id == locationId);

            var summaries = from item in items
                   select item.CreateSummary();

            return ListResponse<DeviceSummary>.Create(summaries);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForOrgIdAsync(DeviceRepository deviceRepo, string orgId, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.OwnerOrganization.Id == orgId &&
                                               qry.DeviceRepository.Id == deviceRepo.Id);

            var summaries = from item in items
                            select item.CreateSummary();

            return ListResponse<DeviceSummary>.Create(summaries);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.Status.Id == status);

            var summaries = from item in items
                            select item.CreateSummary();

            return ListResponse<DeviceSummary>.Create(summaries);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.DeviceConfiguration.Id == configurationId);

            var summaries = from item in items
                            select item.CreateSummary();

            return ListResponse<DeviceSummary>.Create(summaries);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.DeviceType.Id == deviceTypeId);

            var summaries = from item in items
                            select item.CreateSummary();

            return ListResponse<DeviceSummary>.Create(summaries);
        }

        public async  Task<ListResponse<Device>> GetFullDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest listRequest)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            return ListResponse<Device>.Create(await  base.QueryAsync(qry => qry.DeviceConfiguration.Id == configurationId));
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusAsync(DeviceRepository deviceRepo, string customStatus, ListRequest listRequest)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.CustomStatus != null && qry.CustomStatus.Id == customStatus);

            var summaries = from item in items
                            select item.CreateSummary();

            return ListResponse<DeviceSummary>.Create(summaries);
        }
    }
}
