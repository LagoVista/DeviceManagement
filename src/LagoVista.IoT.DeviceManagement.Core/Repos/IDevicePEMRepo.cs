// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e6ca61660b95ae63a9f5e918eb3a7cab7562acbc6d4160ac1e0fa43c8703f4df
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDevicePEMRepo
    {
        Task<string> GetPEMAsync(DeviceRepository deviceRepo, string deviceId, string messageId);

        Task<ListResponse<PEMIndex>> GetPEMIndexForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request);
        Task<ListResponse<PEMIndex>> GetPEMIndexForErrorReasonAsync(DeviceRepository deviceRepo, string errorReason, ListRequest request);
    }
}
