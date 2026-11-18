using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceProximityObservationRepo
    {
        Task WriteAsync(IReadOnlyList<DeviceProximityObservation> observations);
        Task<ListResponse<DeviceProximityObservation>> QueryAsync(string organizationId, string deviceRepoId, string deviceUniqueId, ListRequest request);
    }
}
