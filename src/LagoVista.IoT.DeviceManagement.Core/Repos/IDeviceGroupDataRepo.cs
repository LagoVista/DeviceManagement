using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using LagoVista.IoT.DeviceManagement.Models;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDeviceGroupDataRepo
    {
        Task<ListResponse<DeviceSummaryData>> GetSummaryDataByGroup(DeviceRepository deviceRepo, string groupId);
    }
}
