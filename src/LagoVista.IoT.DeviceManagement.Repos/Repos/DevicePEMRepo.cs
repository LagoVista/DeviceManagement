using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    //TODO: Eventually we need to have this talk to remote devices for cloud version

    public class DevicePEMRepo : IDevicePEMRepo
    {
        IAdminLogger _adminLogger;

        public DevicePEMRepo(IAdminLogger adminLogger)
        {
            _adminLogger = adminLogger;
        }

        private CloudTable GetCloudTable(DeviceRepository deviceRepo)
        {
            var credentials = new StorageCredentials(deviceRepo.PEMStorageSettings.AccountId, deviceRepo.PEMStorageSettings.AccessKey);
            var account = new CloudStorageAccount(credentials, true);
            var tableClient = account.CreateCloudTableClient();
            return tableClient.GetTableReference(deviceRepo.GetPEMStorageName());
        }

        public async Task<string> GetPEMAsync(DeviceRepository deviceRepo, string partitionKey, string rowKey)
        {
            var pems = GetCloudTable(deviceRepo);
        
            var result = await pems.ExecuteAsync(TableOperation.Retrieve<PEMIndex>(partitionKey, rowKey));

            return (result.Result != null) ? (result.Result as PEMIndex).JSON : (string)null;
        }

        public async Task<ListResponse<IPEMIndex>> GetPEMIndexForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            try
            {
                var pems = GetCloudTable(deviceRepo);

                var query = new TableQuery<PEMIndex>()
                    .Where(TableQuery.GenerateFilterCondition(nameof(PEMIndex.PartitionKey), QueryComparisons.Equal, deviceId))
                    .Select(new List<string> { nameof(PEMIndex.RowKey), nameof(PEMIndex.Status), nameof(PEMIndex.CreatedTimeStamp), nameof(PEMIndex.DeviceId), nameof(PEMIndex.MessageId), nameof(PEMIndex.MessageType),
                        nameof(PEMIndex.TotalProcessingMS), nameof(PEMIndex.ErrorReason) });

                var results = await pems.ExecuteQuerySegmentedAsync<PEMIndex>(query, new TableContinuationToken()
                {
                    NextPartitionKey = request.NextPartitionKey,
                    NextRowKey = request.NextRowKey,
                });


                var response = ListResponse<IPEMIndex>.Create(results.ToList());
                if (results.ContinuationToken != null)
                {
                    response.NextPartitionKey = results.ContinuationToken.NextPartitionKey;
                    response.NextRowKey = results.ContinuationToken.NextRowKey;
                    response.HasMoreRecords = !String.IsNullOrEmpty(results.ContinuationToken.NextRowKey);
                }

                return response;
            }
            catch (Exception)
            {
                /* It's possible the table does not exists if it doesn't no data was ever written to list would be empty anyways...return empty list */
                return ListResponse<IPEMIndex>.Create(new List<PEMIndex>());
            }
        }

        public async Task<ListResponse<IPEMIndex>> GetPEMIndexForErrorReasonAsync(DeviceRepository deviceRepo, string errorReason, ListRequest request)
        {
            try
            {
                var pems = GetCloudTable(deviceRepo);

                var query = new TableQuery<PEMIndex>()
                    .Where(TableQuery.GenerateFilterCondition(nameof(PEMIndex.PartitionKey), QueryComparisons.Equal, errorReason))
                    .Select(new List<string> { nameof(PEMIndex.RowKey), nameof(PEMIndex.Status), nameof(PEMIndex.CreatedTimeStamp), nameof(PEMIndex.DeviceId), nameof(PEMIndex.MessageId), nameof(PEMIndex.MessageType),
                        nameof(PEMIndex.TotalProcessingMS), nameof(PEMIndex.ErrorReason) });

                var results = await pems.ExecuteQuerySegmentedAsync<PEMIndex>(query, new TableContinuationToken()
                {
                    NextPartitionKey = request.NextPartitionKey,
                    NextRowKey = request.NextRowKey,
                });

                var response = ListResponse<IPEMIndex>.Create(results.ToList());
                if (results.ContinuationToken != null)
                {
                    response.NextPartitionKey = results.ContinuationToken.NextPartitionKey;
                    response.NextRowKey = results.ContinuationToken.NextRowKey;
                    response.HasMoreRecords = !String.IsNullOrEmpty(results.ContinuationToken.NextRowKey);
                }

                return response;
            }
            catch (Exception)
            {
                /* It's possible the table does not exists if it doesn't no data was ever written to list would be empty anyways...return empty list */
                return ListResponse<IPEMIndex>.Create(new List<PEMIndex>());
            }
        }
    }
}
