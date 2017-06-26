using LagoVista.IoT.DeviceManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceRepositoryRepo
    {
        Task AddDeviceRepositoryAsync(DeviceRepository deviceRepo);

        Task UpdateDeviceRepositoryAsync(DeviceRepository deviceRepo);

        Task<DeviceRepository> GetDeviceRepositoryAsync(DeviceRepository deviceRepo);
    }
}
