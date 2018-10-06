using LagoVista.Core.Validation;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.IO;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    /* These are used to work with actual media items that contain the content/bytes */
    public interface IDeviceMediaRepoRemote
    {
        Task<InvokeResult> AddMediaAsync(DeviceRepository repo, byte[] data, string fileName, string contentType);
    }

    public interface IDeviceMediaRepo : IDeviceMediaRepoRemote
    {
        Task<InvokeResult<byte[]>> GetMediaAsync(DeviceRepository repo, string fileName);

        Task<InvokeResult> DeleteMediaAsync(DeviceRepository repo, string fileName);
    }
}
