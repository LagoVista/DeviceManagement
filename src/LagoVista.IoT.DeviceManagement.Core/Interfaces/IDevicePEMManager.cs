// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 82fcc30a21119467e6e1d27bb5173404300cb066cb0a871834121da2b7c84a74
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDevicePEMManager
    {
        Task<InvokeResult<string>> GetPEMAsync(DeviceRepository deviceRepo, string deviceId, string messageId, EntityHeader org, EntityHeader user);

        Task<ListResponse<PEMIndex>> GetPEMIndexesforDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user);
        Task<ListResponse<PEMIndex>> GetPEMIndexesforErrorReasonAsync(DeviceRepository deviceRepo, string errorReason, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
