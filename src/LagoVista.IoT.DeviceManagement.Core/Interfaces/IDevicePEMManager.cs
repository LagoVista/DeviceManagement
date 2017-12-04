using LagoVista.Core.Models;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDevicePEMManager
    {
        Task<InvokeResult<string>> GetPEMAsync(DeviceRepository deviceRepo, string deviceId, string messageId, EntityHeader org, EntityHeader user);

        Task<ListResponse<IPEMIndex>> GetPEMIndexesforDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user);
        Task<ListResponse<IPEMIndex>> GetPEMIndexesforErrorReasonAsync(DeviceRepository deviceRepo, string errorReason, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
