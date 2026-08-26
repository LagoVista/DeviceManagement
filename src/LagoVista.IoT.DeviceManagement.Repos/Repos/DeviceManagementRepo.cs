// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 144a749c99c8140896a56eff168134be37737d6f95da6f0cfb8771c0e1d2f792
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.CloudStorage;
using LagoVista.CloudStorage.DocumentDB;
using LagoVista.CloudStorage.Interfaces;
using LagoVista.CloudStorage.Storage;
using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core;
using LagoVista.IoT.DeviceManagement.Core.Interfaces;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Core.Resources;
using LagoVista.IoT.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceManagementRepo : DocumentDBRepoBase<Device>, IDeviceManagementRepo
    {
        private const string AZURE_DEVICE_CLIENT_STR = "HostName={0}.azure-devices.net;SharedAccessKeyName={1};SharedAccessKey={2}";

        private readonly IAdminLogger _logger;
        private readonly IDeviceGroupRepo _deviceGroupRepo;
        private readonly IBackgroundServiceTaskQueue _backgroundServiceTaskQueue;
        private readonly IRemoteConfigurationManager _remoteConfigNamanager;

        public DeviceManagementRepo(IBackgroundServiceTaskQueue backgroundServiceTaskQueue,
            IRemoteConfigurationManager remotePropertyManager, IDeviceGroupRepo deviceGroupRepo, IDocumentCloudCachedServices services) : base(services)
        {
            _logger = services.AdminLogger ?? throw new ArgumentNullException(nameof(services.AdminLogger));
            _deviceGroupRepo = deviceGroupRepo ?? throw new ArgumentNullException(nameof(deviceGroupRepo));
            _backgroundServiceTaskQueue = backgroundServiceTaskQueue ?? throw new ArgumentNullException(nameof(backgroundServiceTaskQueue));
            _remoteConfigNamanager = remotePropertyManager ?? throw new ArgumentNullException(nameof(remotePropertyManager));
        }

        public DeviceManagementRepo(IDocumentCloudCachedServices services) : base(services)
        {
        }

        public override string GetPartitionKey()
        {
            return "/DeviceRepository/Id";
        }

        public override String GetCollectionName()
        {
            return "Devices";
        }

        public async Task<InvokeResult> AddDeviceAsync(DeviceRepository deviceRepo, Device device)
        {
            if (String.IsNullOrEmpty(device.Key)) device.Key = device.DeviceId;

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

            if(device.Location != null)
                device.Location.Value = null;

            if(device.DeviceType != null)
                device.DeviceType.Value = null;

            await CreateDocumentAsync(device);

            return InvokeResult.Success;
        }

        public async Task DeleteDeviceAsync(DeviceRepository deviceRepo, string id)
        {
            var device = await GetDeviceByIdAsync(deviceRepo, id);
            await DeleteDocumentAsync(id, deviceRepo.Id);

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var connString = String.Format(AZURE_DEVICE_CLIENT_STR, deviceRepo.ResourceName, deviceRepo.AccessKeyName, deviceRepo.AccessKey);
                var regManager = Microsoft.Azure.Devices.RegistryManager.CreateFromConnectionString(connString);
                await regManager.RemoveDeviceAsync(device.DeviceId);
            }
        }

        public async Task DeleteDeviceByIdAsync(DeviceRepository deviceRepo, string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            if (deviceRepo == null)
            {
                throw new ArgumentNullException(nameof(deviceRepo));
            }

            if (deviceRepo.DeviceStorageSettings == null)
            {
                throw new ArgumentNullException(nameof(deviceRepo.DeviceStorageSettings));
            }

            if (string.IsNullOrEmpty(deviceRepo.DeviceStorageSettings.Uri))
            {
                throw new ArgumentNullException(nameof(deviceRepo.DeviceStorageSettings.Uri));
            }

            if (string.IsNullOrEmpty(deviceRepo.DeviceStorageSettings.AccessKey))
            {
                throw new ArgumentNullException(nameof(deviceRepo.DeviceStorageSettings.AccessKey));
            }

            if (string.IsNullOrEmpty(deviceRepo.DeviceStorageSettings.ResourceName))
            {
                throw new ArgumentNullException(nameof(deviceRepo.DeviceStorageSettings.ResourceName));
            }

            var device = await this.GetDeviceByDeviceIdAsync(deviceRepo, deviceId);
            await DeleteDeviceAsync(deviceRepo, device.Id);

            foreach (var groupEH in device.DeviceGroups)
            {
                var group = await _deviceGroupRepo.GetDeviceGroupAsync(deviceRepo, groupEH.Id);
                var entry = group.Devices.SingleOrDefault(dgr => dgr.DeviceUniqueId == device.Id);
                if (entry != null)
                {
                    group.Devices.Remove(entry);
                    await _deviceGroupRepo.UpdateDeviceGroupAsync(deviceRepo, group);
                    Console.WriteLine("[DeviceManagementRepo__DeleteDeviceByIdAsync]", $"Removed device {device.DeviceId} from group {group.Name} upon device delete.");
                }
                else
                {
                    _logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Warning, "DeviceManagementRepo__DeleteDeviceByIdAsync", $"Could not find associated device in device group for ${device.DeviceId}");
                }


            }

            if (deviceRepo.RepositoryType.Value == RepositoryTypes.AzureIoTHub)
            {
                var connString = String.Format(AZURE_DEVICE_CLIENT_STR, deviceRepo.ResourceName, deviceRepo.AccessKeyName, deviceRepo.AccessKey);
                var regManager = Microsoft.Azure.Devices.RegistryManager.CreateFromConnectionString(connString);
                await regManager.RemoveDeviceAsync(device.DeviceId);
            }
        }

        public async Task<Device> GetDeviceByDeviceIdAsync(DeviceRepository deviceRepo, string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            if (deviceRepo == null)
            {
                throw new ArgumentNullException(nameof(deviceRepo));
            }

            if (deviceRepo.DeviceStorageSettings == null)
            {
                throw new ArgumentNullException(nameof(deviceRepo.DeviceStorageSettings));
            }

            if (string.IsNullOrEmpty(deviceRepo.DeviceStorageSettings.Uri))
            {
                throw new ArgumentNullException(nameof(deviceRepo.DeviceStorageSettings.Uri));
            }

            if (string.IsNullOrEmpty(deviceRepo.DeviceStorageSettings.AccessKey))
            {
                throw new ArgumentNullException(nameof(deviceRepo.DeviceStorageSettings.AccessKey));
            }

            if (string.IsNullOrEmpty(deviceRepo.DeviceStorageSettings.ResourceName))
            {
                throw new ArgumentNullException(nameof(deviceRepo.DeviceStorageSettings.ResourceName));
            }

            var device = (await base.QueryAsync(device => device.DeviceId == deviceId && device.DeviceRepository.Id == deviceRepo.Id)).FirstOrDefault();
            if (device != null)
            {
                foreach (var sensor in device.SensorCollection)
                    sensor.PostLoad();
            }

            return device;
        }

        public async Task<bool> CheckIfDeviceIdInUse(DeviceRepository deviceRepo, string id, string orgid)
        {
            return (await base.QueryAsync(device => device.OwnerOrganization.Id == id && device.DeviceRepository.Id == deviceRepo.Id && device.DeviceId == id)).Any();
        }

        public async Task<Device> GetDeviceByIdAsync(DeviceRepository deviceRepo, string id, bool throwOnRecordNotFound = true)
        {
            if (deviceRepo == null) throw new ArgumentNullException(nameof(deviceRepo));
            if (String.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (deviceRepo.DeviceStorageSettings == null) throw new ArgumentNullException("StorageSettingsOnRepo");

            var device = await GetDocumentAsync(id, deviceRepo.Id, throwOnRecordNotFound);
            if (device != null)
            {
                foreach (var sensor in device.SensorCollection)
                    sensor.PostLoad();
            }

            return device;
        }

        public async Task UpdateDeviceAsync(DeviceRepository deviceRepo, Device device)
        {
            if (String.IsNullOrEmpty(device.Key)) device.Key = device.DeviceId.ToLower();

            /* Make sure that any data that might be sent along with the device but not required is note saved */
            if (device.SensorCollection != null)
            {
                foreach (var sensor in device.SensorCollection)
                {
                    if (!EntityHeader.IsNullOrEmpty(sensor.UnitSet))
                    {
                        sensor.UnitSet.Value = null;
                    }
                }
            }

            var currentConfigLevel = device.DesiredConfigurationRevisionLevel;

            var hash = device.GetConfigurationHash();
            if (device.ConfigurationHash != hash)
            {
                device.ConfigurationHash = hash;
                device.DesiredConfigurationRevisionLevel++;
                device.DesiredConfigurationTimeStamp = UtcTimestamp.Now;

            }

            device.AttributeMetaData = null;
            if (device.Attributes != null)
            {
                foreach (var attribute in device.Attributes)
                {
                    if (!EntityHeader.IsNullOrEmpty(attribute.UnitSet))
                    {
                        attribute.UnitSet.Value = null;
                    }

                    if (!EntityHeader.IsNullOrEmpty(attribute.StateSet))
                    {
                        attribute.StateSet.Value = null;
                    }
                }
            }

            device.DeviceType.Value = null;

            if(device.Location != null)
                device.Location.Value = null;

            await UpsertDocumentAsync(device);

            if (currentConfigLevel != device.DesiredConfigurationRevisionLevel)
            {
                _backgroundServiceTaskQueue.TryQueueBackgroundWorkItem(async (ct) =>
                {
                    await _remoteConfigNamanager.SetDesiredConfigurationRevisionAsync(deviceRepo.Id, device.Id, device.DesiredConfigurationRevisionLevel, device.OwnerOrganization, device.LastUpdatedBy);
                });
            }

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
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.Location.Id == locationId, qry=>qry.Name, request);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForRepositoryAsync(DeviceRepository deviceRepo, string orgId, ListRequest request)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.OwnerOrganization.Id == orgId &&
                                               qry.DeviceRepository.Id == deviceRepo.Id,dev=>dev.Name, request);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForRepositoryAsync(DeviceRepository deviceRepo, string deviceTypeId, string orgId, ListRequest request)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.OwnerOrganization.Id == orgId &&
                                               qry.DeviceRepository.Id == deviceRepo.Id && 
                                               qry.DeviceType.Id == deviceTypeId, dev => dev.Name, request);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, ListRequest request)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.Status.Id == status, dev=>dev.Name, request);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest request)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.DeviceConfiguration.Id == configurationId && qry.DeviceRepository.Id == deviceRepo.Id,
                                              qry => qry.Name, request);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, ListRequest request)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.DeviceType.Id == deviceTypeId && qry.DeviceRepository.Id == deviceRepo.Id, dev=>dev.Name, request);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeKeyAsync(DeviceRepository deviceRepo, string deviceTypeKey, ListRequest request)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.DeviceType.Key == deviceTypeKey && qry.DeviceRepository.Id == deviceRepo.Id, dev => dev.Name, request);
        }

        public async Task<ListResponse<Device>> GetFullDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest listRequest)
        {
            return await base.QueryAsync(qry => qry.DeviceConfiguration.Id == configurationId && qry.DeviceRepository.Id == deviceRepo.Id, dev => dev.Name, listRequest);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusAsync(DeviceRepository deviceRepo, string customStatus, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.DeviceRepository.Id == deviceRepo.Id && qry.CustomStatus != null && qry.CustomStatus.Id == customStatus, dev => dev.Name, listRequest);
        }

        public async Task<ListResponse<DeviceSummary>> SearchByDeviceIdAsync(DeviceRepository deviceRepo, string search, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.DeviceRepository.Id == deviceRepo.Id && qry.DeviceId.Contains(search), dev => dev.Name, listRequest);
        }

        public async Task<ListResponse<DeviceSummaryData>> GetDeviceGroupSummaryDataAsync(DeviceRepository deviceRepo, string groupId, ListRequest listRequest)
        {
            var devices = await QueryAsync(device => device.DeviceRepository != null && device.DeviceRepository.Id == deviceRepo.Id && device.DeviceGroups != null && device.DeviceGroups.Any(group => group.Id == groupId));

            var items = devices.Select(device => DeviceSummaryData.FromDevice(device)).ToList();
            var listResponse = ListResponse<DeviceSummaryData>.Create(items);
            listResponse.PageSize = items.Count;
            listResponse.HasMoreRecords = false;
            listResponse.GetListUrl = listRequest.Url;

            return listResponse;
        }


        public Task<string> Echo(string value)
        {
            return Task.FromResult(value);
        }

        public async Task<ListResponse<DeviceSummary>> GetChildDevicesAsync(DeviceRepository repo, string parentDeviceId, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.ParentDevice != null && qry.ParentDevice.Id == parentDeviceId && qry.DeviceRepository.Id == repo.Id, dev=>dev.Name, listRequest);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForRepositoryForUserAsync(DeviceRepository deviceRepo, string userId, string orgId, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.OwnerOrganization.Id == orgId &&
                                               (qry.AssignedUser != null && qry.AssignedUser.Id == userId) &&
                                               qry.DeviceRepository.Id == deviceRepo.Id, dev=>dev.Name, listRequest);
        }

        public async Task<Device> GetDeviceByMacAddressAsync(DeviceRepository deviceRepo, string macAddress)
        {
            var devices = await base.QueryAsync(qry => qry.OwnerOrganization.Id == deviceRepo.OwnerOrganization.Id &&
                                                       qry.DeviceRepository.Id == deviceRepo.Id &&
                                                       qry.MacAddress == macAddress);
            var device = devices.FirstOrDefault();
            if (device != null)
            {
                foreach (var sensor in device.SensorCollection)
                    sensor.PostLoad();
            }

            return device;
        }

        public async Task<Device> GetDeviceByiOSBLEAddressAsync(DeviceRepository deviceRepo, string iosBLEAddress)
        {
            var devices = await base.QueryAsync(qry => qry.OwnerOrganization.Id == deviceRepo.OwnerOrganization.Id &&
                                                       qry.DeviceRepository.Id == deviceRepo.Id &&
                                                       qry.iosBLEAddress == iosBLEAddress);
            var device = devices.FirstOrDefault();
            if (device != null)
            {
                foreach (var sensor in device.SensorCollection)
                    sensor.PostLoad();
            }

            return device;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForCustomerAsync(DeviceRepository deviceRepo, string orgId, string customerId, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.OwnerOrganization.Id == orgId &&
                                                qry.Customer.Id == customerId &&
                                                qry.DeviceRepository.Id == deviceRepo.Id, dev => dev.Name, listRequest);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForCustomerLocationAsync(DeviceRepository deviceRepo, string orgId, string customerId, string customerLocationId, ListRequest listRequest)
        {
            return await base.QuerySummaryAsync<DeviceSummary, Device>(qry => qry.OwnerOrganization.Id == orgId &&
                                                qry.Customer.Id == customerId && qry.CustomerLocation.Id == customerLocationId &&
                                                qry.DeviceRepository.Id == deviceRepo.Id, dev => dev.Name, listRequest);
        }
    }
}
