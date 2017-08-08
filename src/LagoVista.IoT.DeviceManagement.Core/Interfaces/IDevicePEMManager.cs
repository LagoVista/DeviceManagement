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
        Task<InvokeResult<string>> GetPEMAsync(DeviceRepository deviceRepo, String pemURI, EntityHeader org, EntityHeader user);

        Task<ListResponse<DevicePEMIndex>> GetPEMIndexesforDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request, EntityHeader org, EntityHeader user);
    }
}
