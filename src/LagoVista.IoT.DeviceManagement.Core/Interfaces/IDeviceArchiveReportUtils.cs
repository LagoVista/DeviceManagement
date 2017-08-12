using System.Collections.Generic;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceArchiveReportUtils
    {
        ListResponse<List<object>> CreateNormalizedDeviceArchiveResult(List<Dictionary<string, object>> rows);
    }
}