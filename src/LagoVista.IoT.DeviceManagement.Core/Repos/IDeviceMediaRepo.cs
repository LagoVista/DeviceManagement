// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7c2376371b9f3fd3d9915403c7e74ec62c98d6f3cc28c1b66dc4a08922c6820a
// IndexVersion: 0
// --- END CODE INDEX META ---
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
