using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Core.Repos
{
    public interface IDataStreamRepo
    {
        Task AddDataStreamAsync(DataStream dataStream);

        Task UpdateDataStreamAsync(DataStream dataStream);

        Task<DataStream> GetDataStreamAsync(string id);

        Task<IEnumerable<DataStreamSummary>> GetDataStreamsForOrgAsync(string orgId);

        Task DeleteDataStreamAsync(string dataStreamId);

        Task<bool> QueryKeyInUseAsync(string key, string orgId);
    }
}
