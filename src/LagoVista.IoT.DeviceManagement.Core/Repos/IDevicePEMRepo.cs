using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDevicePEMRepo
    {
        Task<string> GetPEMAsync(DeviceRepository deviceRepo, String pemURI);
    }
}
