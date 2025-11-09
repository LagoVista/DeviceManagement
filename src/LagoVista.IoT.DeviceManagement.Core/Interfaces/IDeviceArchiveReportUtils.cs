// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f9a4355ab6c3044f8b192c9186899b772ed7ede4d089193156884ac3f05a80a7
// IndexVersion: 2
// --- END CODE INDEX META ---
using System.Collections.Generic;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.IoT.DeviceManagement.Core
{
    public interface IDeviceArchiveReportUtils
    {
        ListResponse<List<object>> CreateNormalizedDeviceArchiveResult(List<Dictionary<string, object>> rows);
    }
}