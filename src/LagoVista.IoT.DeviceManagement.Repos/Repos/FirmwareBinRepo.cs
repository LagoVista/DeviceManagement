using LagoVista.Core;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class FirmwareBinRepo
    {
        private readonly IAdminLogger _logger;
        private readonly IConnectionSettings _connectionSettings;

        public FirmwareBinRepo(IAdminLogger adminLogger, IConnectionSettings connectionSettings)
        {
            _logger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
            _connectionSettings = connectionSettings ?? throw new ArgumentNullException(nameof(connectionSettings));
        }

        private CloudBlobClient CreateBlobClient()
        {
            var baseuri = $"https://{_connectionSettings.AccountId}.blob.core.windows.net";

            var uri = new Uri(baseuri);
            return new CloudBlobClient(uri, new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(_connectionSettings.AccountId, _connectionSettings.AccessKey));
        }

        private async Task<InvokeResult<CloudBlobContainer>> GetStorageContainerAsync( string containerName)
        {
            var client = CreateBlobClient();
            var container = client.GetContainerReference(containerName);
            try
            {
                var options = new BlobRequestOptions()
                {
                    MaximumExecutionTime = TimeSpan.FromSeconds(15)
                };

                var opContext = new OperationContext();
                await container.CreateIfNotExistsAsync(options, opContext);
                return InvokeResult<CloudBlobContainer>.Create(container);
            }
            catch (ArgumentException ex)
            {
                _logger.AddException("[FirmwareBinRepo_GetStorageContainerAsync]", ex);
                return InvokeResult<CloudBlobContainer>.FromException("[FirmwareBinRepo_GetStorageContainerAsync_InitAsync]", ex);
            }
            catch (StorageException ex)
            {
                _logger.AddException("[FirmwareBinRepo_GetStorageContainerAsync", ex);
                return InvokeResult<CloudBlobContainer>.FromException("[FirmwareBinRepo_GetStorageContainerAsync]", ex);
            }
        }

        public async Task<InvokeResult> AddBinAsync(byte[] data, string fileName)
        {
            var result = await GetStorageContainerAsync("firmware");
            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var container = result.Result;

            var blob = container.GetBlockBlobReference(fileName);
            blob.Properties.ContentType = "application/octet-stream";

            //TODO: Should really encapsulate the idea of retry of an action w/ error reporting
            var numberRetries = 5;
            var retryCount = 0;
            var completed = false;
            var stream = new MemoryStream(data);
            while (retryCount++ < numberRetries && !completed)
            {
                try
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    await blob.UploadFromStreamAsync(stream);
                }
                catch (Exception ex)
                {
                    if (retryCount == numberRetries)
                    {
                        _logger.AddException("FirmwareBinRepo_AddItemAsync", ex);
                        return InvokeResult.FromException("FirmwareBinRepo_AddItemAsync", ex);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Warning, "FirmwareBinRepo_AddItemAsync", "", ex.Message.ToKVP("exceptionMessage"), ex.GetType().Name.ToKVP("exceptionType"), retryCount.ToString().ToKVP("retryCount"));
                    }
                    await Task.Delay(retryCount * 250);
                }
            }

            return InvokeResult.Success;
        }

        public async Task<InvokeResult<byte[]>> GetFirmwareBinaryAsync(string fileName)
        {
            var result = await GetStorageContainerAsync("firmware");
            if (!result.Successful)
            {
                return InvokeResult<byte[]>.FromInvokeResult(result.ToInvokeResult());
            }

            var container = result.Result;

            var blob = container.GetBlockBlobReference(fileName);
            var numberRetries = 5;
            var retryCount = 0;
            var completed = false;
            while (retryCount++ < numberRetries && !completed)
            {
                try
                {
                    //TODO: We shouldn't likely return a byte array here probably return access to the response object and stream the bytes as they are downloaded, current architecture doesn't support...
                    using (var ms = new MemoryStream())
                    {
                        await blob.DownloadToStreamAsync(ms);
                        return InvokeResult<byte[]>.Create(ms.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    if (retryCount == numberRetries)
                    {
                        _logger.AddException("FirmwareBinRepo_GetFirmwareBinaryAsync", ex);
                        return InvokeResult<byte[]>.FromException("FirmwareBinRepo_GetFirmwareBinaryAsync", ex);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Warning, "FirmwareBinRepo_GetFirmwareBinaryAsync", "", fileName.ToKVP("fileName"),
                            ex.Message.ToKVP("exceptionMessage"), ex.GetType().Name.ToKVP("exceptionType"), retryCount.ToString().ToKVP("retryCount"));
                    }
                    await Task.Delay(retryCount * 250);
                }
            }

            return InvokeResult<byte[]>.FromError("Could not retrieve fimrware binary");
        }
    }
}
