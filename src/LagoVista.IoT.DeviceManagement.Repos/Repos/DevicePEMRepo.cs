using LagoVista.IoT.DeviceManagement.Core.Repos;
using LagoVista.IoT.Logging.Loggers;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Repos.Repos
{
    public class DevicePEMRepo : IDevicePEMRepo
    {
        CloudBlobClient _cloudClient;
        private String _baseURI;
        
        IAdminLogger _adminLogger;

        public DevicePEMRepo(IDeviceManagementSettings connectionSettings, IAdminLogger adminLogger)
        {
            _adminLogger = adminLogger;

            _baseURI = $"https://{connectionSettings.PEMStorage.AccountId}.blob.core.windows.net";

            var uri = new Uri(_baseURI);
            _cloudClient = new CloudBlobClient(uri, new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(connectionSettings.PEMStorage.AccountId, connectionSettings.PEMStorage.AccessKey));
        }

        private string ContainerReferenceId { get { return $"pem"; } }

        public async Task<string> GetPEMAsync(string pemURI)
        {
            var container = _cloudClient.GetContainerReference(ContainerReferenceId);

            var corePath = $"{_baseURI}/{ContainerReferenceId}/".ToLower();

            /* 
             * The method is passed in the full  URI of the BLOB we need to strip
             * out the end point and the container to get the name of the blob
             */
            var blobName = pemURI.ToLower().Replace(corePath, "");
            var blob = container.GetBlockBlobReference(blobName);

            return await blob.DownloadTextAsync();
        }
    }
}
