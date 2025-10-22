// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 9b86538fcc8c3b71329762f80fcdee81fe2521abb663f87120053c10bfcbf967
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IFirmwareDownloadManager
    {
        Task<InvokeResult<FirmwareDownload>> DownloadFirmwareAsync(string type, string downloadId, int? startIndex = null, int? length = null);
        Task<InvokeResult<int>> GetFirmwareLengthAsync(string downloadId);
        Task<InvokeResult> MarkAsCompleteAsync(string downloadId);
        Task<InvokeResult> MarkAsFailedAsync(string downloadId, string err);
    }

    public interface IFirmwareManager
    {
        Task<InvokeResult> AddFirmwareAsync(Firmware firmware, EntityHeader org, EntityHeader user);
        Task<Firmware> GetFirmwareAsync(string id, EntityHeader org, EntityHeader user);
        Task<ListResponse<FirmwareSummary>> GetFirmwareForOrgAsync(string orgId, EntityHeader user, ListRequest listRequest);
        Task<ListResponse<FirmwareDownloadRequest>> GetRequestsForDeviceAsync(string deviceRepoId, string deviceId, EntityHeader user, EntityHeader org, ListRequest listRequest);
        Task<InvokeResult> UpdateFirmwareAsync(Firmware firmware, EntityHeader org, EntityHeader user);
        Task<InvokeResult> DeleteFirmwareAsync(string id, EntityHeader org, EntityHeader user);
        Task<bool> QueryKeyInUse(string key, EntityHeader org);
        Task<InvokeResult<EntityHeader>> UploadMainRevision(string firmwareId, string revisionid, Stream stream, EntityHeader org, EntityHeader user);
        Task<InvokeResult<EntityHeader>> UploadOtaRevision(string firmwareId, string revisionid, Stream stream, EntityHeader org, EntityHeader user);
        Task<InvokeResult<FirmwareDownloadRequest>> RequestDownloadLinkAsync(string repoId, string deviceId, string firmwareId, string revisionId, EntityHeader org, EntityHeader user);
        Task<InvokeResult<byte[]>> DownloadFirmwareAsync(string type, string firmwareId, string revisionId, EntityHeader org, EntityHeader user);
        
       
    }
}
