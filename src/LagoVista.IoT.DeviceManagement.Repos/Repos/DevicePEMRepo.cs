using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using System.Linq;
using System.Threading.Tasks;
using LagoVista.Core.Models.UIMetaData;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using LagoVista.Core;
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

            var result = await pems.ExecuteAsync(TableOperation.Retrieve<PEMIndexDTO>(partitionKey, rowKey));

            if (result.Result is PEMIndexDTO pemIndex)
            {
                var pemResult = pemIndex.ToPEM();
                if (pemResult.Successful)
                {
                    return pemResult.Result;
                }
                else
                {
                    _adminLogger.AddError("DevicePEMRepo_GetPEMAsync", pemResult.Errors.First().Message);
                    return null;
                }
            }
            else
            {
                _adminLogger.AddError("DevicePEMRepo_GetPEMAsync", "Null response from TableStorage", rowKey.ToKVP("rowKey"), partitionKey.ToKVP("partitionKey"),
                                        deviceRepo.OwnerOrganization.Text.ToKVP("org"), deviceRepo.Key.ToKVP("repo"));
                return null;
            }
        }

        public async Task<ListResponse<PEMIndex>> GetPEMIndexForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request)
        {
            try
            {
                var pems = GetCloudTable(deviceRepo);

                var query = new TableQuery<PEMIndexDTO>()
                    .Where(TableQuery.GenerateFilterCondition(nameof(PEMIndexDTO.PartitionKey), QueryComparisons.Equal, deviceId))
                    .Select(new List<string>
                    {
                        nameof(PEMIndexDTO.RowKey),
                        nameof(PEMIndexDTO.Status),
                        nameof(PEMIndexDTO.CreatedTimeStamp),
                        nameof(PEMIndexDTO.DeviceId),
                        nameof(PEMIndexDTO.MessageId),
                        nameof(PEMIndexDTO.MessageType),
                        nameof(PEMIndexDTO.TotalProcessingMS),
                        nameof(PEMIndexDTO.ErrorReason),
                        nameof(PEMIndexDTO.SolutionVersion),
                        nameof(PEMIndexDTO.RuntimeVersion)})
                        .Take(request.PageSize);

                var results = await pems.ExecuteQuerySegmentedAsync<PEMIndexDTO>(query, new TableContinuationToken()
                {
                    NextPartitionKey = request.NextPartitionKey,
                    NextRowKey = request.NextRowKey,
                });


                var response = ListResponse<PEMIndex>.Create(results.Select(pem=>pem.ToPEMIndex()).ToList());
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
                return ListResponse<PEMIndex>.Create(new List<PEMIndex>());
            }
        }

        public async Task<ListResponse<PEMIndex>> GetPEMIndexForErrorReasonAsync(DeviceRepository deviceRepo, string errorReason, ListRequest request)
        {
            try
            {
                var pems = GetCloudTable(deviceRepo);

                var query = new TableQuery<PEMIndexDTO>()
                    .Where(TableQuery.GenerateFilterCondition(nameof(PEMIndexDTO.PartitionKey), QueryComparisons.Equal, errorReason))
                    .Select(new List<string> { 
                        nameof(PEMIndexDTO.RowKey), 
                        nameof(PEMIndexDTO.Status), 
                        nameof(PEMIndexDTO.CreatedTimeStamp),
                        nameof(PEMIndexDTO.DeviceId),
                        nameof(PEMIndexDTO.MessageId), 
                        nameof(PEMIndexDTO.MessageType),
                        nameof(PEMIndexDTO.TotalProcessingMS), 
                        nameof(PEMIndexDTO.ErrorReason),
                        nameof(PEMIndexDTO.SolutionVersion),
                        nameof(PEMIndexDTO.RuntimeVersion) })
                        .Take(request.PageSize);

                var results = await pems.ExecuteQuerySegmentedAsync<PEMIndexDTO>(query, new TableContinuationToken()
                {
                    NextPartitionKey = request.NextPartitionKey,
                    NextRowKey = request.NextRowKey,
                });

                var response = ListResponse<PEMIndex>.Create(results.Select(pem=>pem.ToPEMIndex()).ToList());
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
                return ListResponse<PEMIndex>.Create(new List<PEMIndex>());
            }
        }
    }
}
