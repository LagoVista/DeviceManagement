using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Managers
{
    public interface IDevicePEMManager
    {
        Task<InvokeResult<string>> GetPEMAsync(String pemURI, EntityHeader org, EntityHeader user);

        Task<IEnumerable<DevicePEMIndex>> GetPEMIndexesforDeviceAsync(string deviceId, EntityHeader org, EntityHeader user, int maxReturnCount = 100, String returnAfter = "");
    }
}
