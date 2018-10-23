using LagoVista.CloudStorage.DocumentDB;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.DeviceManagement.Core.Resources;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using LagoVista.Core;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceManagementRepo : DocumentDBRepoBase<Device>, IDeviceManagementRepo
    {
        private const string AZURE_DEVICE_CLIENT_STR = "HostName={0}.azure-devices.net;SharedAccessKeyName={1};SharedAccessKey={2}";

        private bool _shouldConsolidateCollections;
        IAdminLogger _logger;

        public DeviceManagementRepo(IDeviceManagementSettings repoSettings, IAdminLogger logger) : base(logger)
        {
            _shouldConsolidateCollections = repoSettings.ShouldConsolidateCollections;
            _logger = logger;
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

            var items = await base.QueryAsync(qry => qry.Location.Id == locationId, request);

            var summaries = from item in items.Model
                            select item.CreateSummary();

            var lr = ListResponse<DeviceSummary>.Create(summaries);
            lr.NextPartitionKey = items.NextPartitionKey;
            lr.NextRowKey = items.NextRowKey;
            lr.PageSize = items.PageSize;
            lr.HasMoreRecords = items.HasMoreRecords;
            lr.PageIndex = items.PageIndex;
            return lr;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesForRepositoryAsync(DeviceRepository deviceRepo, string orgId, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.OwnerOrganization.Id == orgId &&
                                               qry.DeviceRepository.Id == deviceRepo.Id, request);

            var summaries = from item in items.Model
                            select item.CreateSummary();

            var lr = ListResponse<DeviceSummary>.Create(summaries);
            lr.NextPartitionKey = items.NextPartitionKey;
            lr.NextRowKey = items.NextRowKey;
            lr.PageSize = items.PageSize;
            lr.HasMoreRecords = items.HasMoreRecords;
            lr.PageIndex = items.PageIndex;
            return lr;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesInStatusAsync(DeviceRepository deviceRepo, string status, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.Status.Id == status, request);

            var summaries = from item in items.Model
                            select item.CreateSummary();

            var lr = ListResponse<DeviceSummary>.Create(summaries);
            lr.NextPartitionKey = items.NextPartitionKey;
            lr.NextRowKey = items.NextRowKey;
            lr.PageSize = items.PageSize;
            lr.HasMoreRecords = items.HasMoreRecords;
            lr.PageIndex = items.PageIndex;
            return lr;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.DeviceConfiguration.Id == configurationId && qry.DeviceRepository.Id == deviceRepo.Id, request);

            var summaries = from item in items.Model
                            select item.CreateSummary();

            var lr = ListResponse<DeviceSummary>.Create(summaries);
            lr.NextPartitionKey = items.NextPartitionKey;
            lr.NextRowKey = items.NextRowKey;
            lr.PageSize = items.PageSize;
            lr.HasMoreRecords = items.HasMoreRecords;
            lr.PageIndex = items.PageIndex;
            return lr;
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesWithDeviceTypeAsync(DeviceRepository deviceRepo, string deviceTypeId, ListRequest request)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.DeviceType.Id == deviceTypeId && qry.DeviceRepository.Id == deviceRepo.Id, request);

            var summaries = from item in items.Model
                            select item.CreateSummary();

            var lr = ListResponse<DeviceSummary>.Create(summaries);
            lr.NextPartitionKey = items.NextPartitionKey;
            lr.NextRowKey = items.NextRowKey;
            lr.PageSize = items.PageSize;
            lr.HasMoreRecords = items.HasMoreRecords;
            lr.PageIndex = items.PageIndex;
            return lr;
        }

        public async Task<ListResponse<Device>> GetFullDevicesWithConfigurationAsync(DeviceRepository deviceRepo, string configurationId, ListRequest listRequest)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            return await base.QueryAsync(qry => qry.DeviceConfiguration.Id == configurationId && qry.DeviceRepository.Id == deviceRepo.Id, listRequest);
        }

        public async Task<ListResponse<DeviceSummary>> GetDevicesInCustomStatusAsync(DeviceRepository deviceRepo, string customStatus, ListRequest listRequest)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.DeviceRepository.Id == deviceRepo.Id && qry.CustomStatus != null && qry.CustomStatus.Id == customStatus, listRequest);

            var summaries = from item in items.Model
                            select item.CreateSummary();

            var lr = ListResponse<DeviceSummary>.Create(summaries);
            lr.NextPartitionKey = items.NextPartitionKey;
            lr.NextRowKey = items.NextRowKey;
            lr.PageSize = items.PageSize;
            lr.HasMoreRecords = items.HasMoreRecords;
            lr.PageIndex = items.PageIndex;
            return lr;
        }

        public async Task<ListResponse<DeviceSummary>> SearchByDeviceIdAsync(DeviceRepository deviceRepo, string search, ListRequest listRequest)
        {
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.DeviceRepository.Id == deviceRepo.Id && qry.DeviceId.Contains(search), listRequest);

            var summaries = from item in items.Model
                            select item.CreateSummary();

            var lr = ListResponse<DeviceSummary>.Create(summaries);
            lr.NextPartitionKey = items.NextPartitionKey;
            lr.NextRowKey = items.NextRowKey;
            lr.PageSize = items.PageSize;
            lr.HasMoreRecords = items.HasMoreRecords;
            lr.PageIndex = items.PageIndex;
            return lr;
        }

        public async Task<ListResponse<DeviceSummaryData>> GetDeviceGroupSummaryDataAsync(DeviceRepository deviceRepo, string groupId, ListRequest listRequest)
        {            
            SetConnection(deviceRepo.DeviceStorageSettings.Uri, deviceRepo.DeviceStorageSettings.AccessKey, deviceRepo.DeviceStorageSettings.ResourceName);
            
            var options = new FeedOptions()
            {
                MaxItemCount = (listRequest.PageSize == 0) ? 50 : listRequest.PageSize
            };

            if (!String.IsNullOrEmpty(listRequest.NextRowKey))
            {
                options.RequestContinuation = listRequest.NextRowKey;
            }


            var query = @"SELECT c.id, c.Status, c.Speed, c.Heading, c.Name, c.DeviceType, c.DeviceConfiguration, c.DeviceRepository,
   c.DeviceId, c.Attributes, c.States, c.Properties, c.GeoLocation FROM c 
  join d in c.DeviceGroups 
  where c.EntityType = 'Device' 
   and c.DeviceRepository.Id = @repodid
   and d.Id = @groupid";

            var sqlParams = new SqlParameterCollection();
            sqlParams.Add(new SqlParameter("@repodid", deviceRepo.Id));
            sqlParams.Add(new SqlParameter("@groupid", groupId));

            try
            {
                var spec = new SqlQuerySpec(query, sqlParams);
                var link = await GetCollectionDocumentsLinkAsync();

                var docQuery = Client.CreateDocumentQuery<DeviceSummaryData>(link, spec, options).AsDocumentQuery();
                var result = await docQuery.ExecuteNextAsync<DeviceSummaryData>();

                var listResponse = ListResponse<DeviceSummaryData>.Create(result);
                listResponse.NextRowKey = result.ResponseContinuation;
                listResponse.PageSize = result.Count;
                listResponse.HasMoreRecords = result.Count == listRequest.PageSize;
                listResponse.PageIndex = listRequest.PageIndex;

                return listResponse;
            }
            catch(Exception ex)
            {
                _logger.AddException("DeviceManagementRepo_GetDeviceGroupSummaryDataAsync", ex, typeof(DeviceSummaryData).Name.ToKVP("entityType"), groupId.ToKVP("groupId"), deviceRepo.Id.ToKVP("deviceRepoId"));

                var listResponse = ListResponse<DeviceSummaryData>.Create(new List<DeviceSummaryData>());
                listResponse.Errors.Add(new ErrorMessage(ex.Message));
                return listResponse;

            }
        }

        public Task<string> Echo(string value)
        {
            return Task.FromResult(value);
        }

        public async Task<ListResponse<DeviceSummary>> GetChildDevicesAsync(DeviceRepository repo, string parentDeviceId, ListRequest listRequest)
        {
            SetConnection(repo.DeviceStorageSettings.Uri, repo.DeviceStorageSettings.AccessKey,repo.DeviceStorageSettings.ResourceName);

            var items = await base.QueryAsync(qry => qry.ParentDevice != null && qry.ParentDevice.Id == parentDeviceId && qry.DeviceRepository.Id == repo.Id, listRequest);

            var summaries = from item in items.Model
                            select item.CreateSummary();

            var lr = ListResponse<DeviceSummary>.Create(summaries);
            lr.NextPartitionKey = items.NextPartitionKey;
            lr.NextRowKey = items.NextRowKey;
            lr.PageSize = items.PageSize;
            lr.HasMoreRecords = items.HasMoreRecords;
            lr.PageIndex = items.PageIndex;
            return lr;
        }
    }
}
