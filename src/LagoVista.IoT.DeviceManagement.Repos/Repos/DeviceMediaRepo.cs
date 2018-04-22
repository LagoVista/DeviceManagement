using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Core.Repos;
using System;
using LagoVista.Core;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using LagoVista.Core.Interfaces;
using Microsoft.WindowsAzure.Storage;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core.PlatformSupport;
using System.IO;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DeviceMediaRepo : IDeviceMediaRepo
    {
        ILogger _logger;

        public DeviceMediaRepo(IAdminLogger adminLogger)
        {
            _logger = adminLogger;
        }

        public DeviceMediaRepo(IInstanceLogger instanceLogger)
        {
            _logger = instanceLogger;
        }

        private CloudBlobClient CreateBlobClient(IConnectionSettings settings)
        {
            var baseuri = $"https://{settings.AccountId}.blob.core.windows.net";

            var uri = new Uri(baseuri);
            return new CloudBlobClient(uri, new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(settings.AccountId, settings.AccessKey));
        }

        private async Task<InvokeResult<CloudBlobContainer>> GetStorageContainerAsync(IConnectionSettings settings, string containerName)
        {
            var client = CreateBlobClient(settings);
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
                _logger.AddException("DeviceMediaRepo_GetStorageContainerAsync", ex);
                return InvokeResult<CloudBlobContainer>.FromException("DeviceMediaRepo_GetStorageContainerAsync_InitAsync", ex);
            }
            catch (StorageException ex)
            {
                _logger.AddException("DeviceMediaRepo_GetStorageContainerAsync", ex);
                return InvokeResult<CloudBlobContainer>.FromException("DeviceMediaRepo_GetStorageContainerAsync", ex);
            }
        }


        public async Task<InvokeResult> AddMediaAsync(DeviceRepository repo, Stream stream, string fileName, string contentType)
        {
            var result = await GetStorageContainerAsync(repo.DeviceArchiveStorageSettings, repo.GetDeviceMediaStorageName());
            if (!result.Successful)
            {
                return result.ToInvokeResult();
            }

            var container = result.Result;

            var blob = container.GetBlockBlobReference(fileName);
            blob.Properties.ContentType = contentType;

            //TODO: Should really encapsulate the idea of retry of an action w/ error reporting
            var numberRetries = 5;
            var retryCount = 0;
            var completed = false;
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
                        _logger.AddException("DeviceMediaRepo_AddItemAsync", ex);
                        return InvokeResult.FromException("DeviceMediaRepo_AddItemAsync", ex);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Warning, "DeviceMediaRepo_AddItemAsync", "", ex.Message.ToKVP("exceptionMessage"), ex.GetType().Name.ToKVP("exceptionType"), retryCount.ToString().ToKVP("retryCount"));
                    }
                    await Task.Delay(retryCount * 250);
                }
            }

            return InvokeResult.Success;

        }

        public async Task<InvokeResult> DeleteMediaAsync(DeviceRepository repo, string fileName)
        {
            var result = await GetStorageContainerAsync(repo.DeviceArchiveStorageSettings, repo.GetDeviceMediaStorageName());
            if (!result.Successful)
            {
                return result.ToInvokeResult();
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
                    await blob.DeleteIfExistsAsync();
                }
                catch (Exception ex)
                {
                    if (retryCount == numberRetries)
                    {
                        _logger.AddException("DeviceMediaRepo_DeleteMediaAsync", ex);
                        return InvokeResult.FromException("AzureBlobConnector_DeleteMediaAsync", ex);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Warning, "DeviceMediaRepo_DeleteMediaAsync", "", ex.Message.ToKVP("exceptionMessage"), ex.GetType().Name.ToKVP("exceptionType"), retryCount.ToString().ToKVP("retryCount"));
                    }
                    await Task.Delay(retryCount * 250);
                }
            }

            return InvokeResult.Success;
        }

        public async Task<InvokeResult<byte[]>> GetMediaAsync(DeviceRepository repo, string fileName)
        {
            var result = await GetStorageContainerAsync(repo.DeviceArchiveStorageSettings, repo.GetDeviceMediaStorageName());
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
                        _logger.AddException("DeviceMediaRepo_GetMediAsync", ex);
                        return InvokeResult<byte[]>.FromException("DeviceMediaRepo_GetMediAsync", ex);
                    }
                    else
                    {
                        _logger.AddCustomEvent(LagoVista.Core.PlatformSupport.LogLevel.Warning, "DeviceMediaRepo_GetMediAsync", "", ex.Message.ToKVP("exceptionMessage"), ex.GetType().Name.ToKVP("exceptionType"), retryCount.ToString().ToKVP("retryCount"));
                    }
                    await Task.Delay(retryCount * 250);
                }
            }

            return InvokeResult<byte[]>.FromError("Could not retrieve Media Item");
        }
    }
}
