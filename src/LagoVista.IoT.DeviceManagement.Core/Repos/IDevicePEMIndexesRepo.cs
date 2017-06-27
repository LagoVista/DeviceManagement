using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDevicePEMIndexesRepo
    {
        Task<IEnumerable<DevicePEMIndex>> GetPEMIndexForDeviceAsync(DeviceRepository deviceRepo, string deviceId, int take, string dateStampAfter);
    }
}