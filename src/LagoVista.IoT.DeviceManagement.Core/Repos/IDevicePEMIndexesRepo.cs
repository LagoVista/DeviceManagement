using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDevicePEMIndexesRepo
    {
        Task<ListResponse<DevicePEMIndex>> GetPEMIndexForDeviceAsync(DeviceRepository deviceRepo, string deviceId, ListRequest request);
    }
}