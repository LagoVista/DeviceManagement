using LagoVista.Core.Models.UIMetaData;
using LagoVista.IoT.DeviceManagement.Core.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using LagoVista.Core;

namespace LagoVista.IoT.DeviceManagement.Core.Reporting
{
    public class DeviceArchiveReportUtils : IDeviceArchiveReportUtils
    {
        /// <summary>
        /// This method will take a result table storage with set of rows that contains different result sets, it will then
        /// normalize the out put to replace missing columns with nulls.
        /// 
        /// The result will be similar to
        /// 
        /// 
        /// Record Type, Col1, Col2, Col3, Col4, Col4
        /// ===============================================
        /// Type    A     X     X
        /// Type    A     X     X
        /// Type    B                  X    X
        /// Type    A     X     X
        /// Type    C                       X     X
        /// Type    B                  X    X
        /// Type    A     X     X
        /// Type    C                       X     X        
        /// 
        /// Bottom line is when results are returned each row
        /// will have the exact same number of columns and 
        /// values in the proper columns
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public ListResponse<List<object>> CreateNormalizedDeviceArchiveResult(List<Dictionary<string, object>> rows)
        {
            var listResponse = new ListResponse<List<object>>();
            var resultRows = new List<List<object>>();

            var columns = new List<ListColumn>();
            columns.Add(new ListColumn()
            {
                FieldName = "Timestamp",
                Header = "Timestamp"
            });

            columns.Add(new ListColumn()
            {
                FieldName = "PEMMessageId",
                Header = "PEMMessageId"
            });


            foreach (var row in rows)
            {
                var archive = new List<object>();

                row.Remove(nameof(DeviceArchive.RowKey));
                row.Remove(nameof(DeviceArchive.PartitionKey));
                row.Remove("odata.etag");
                row.Remove(nameof(DeviceArchive.DeviceId));
                row.Remove(nameof(DeviceArchive.DeviceConfigurationVersionId));
                row.Remove(nameof(DeviceArchive.DeviceConfigurationId));
                var timeStamp = row[nameof(DeviceArchive.Timestamp)].ToString();
                var dateTime = DateTime.Parse(timeStamp, null, System.Globalization.DateTimeStyles.AssumeUniversal);
                archive.Add(dateTime.ToJSONString());
                row.Remove(nameof(DeviceArchive.Timestamp));
                archive.Add(row[nameof(DeviceArchive.PEMMessageId)].ToString());
                row.Remove(nameof(DeviceArchive.PEMMessageId));
                
                /* Build up template of empty columns entries */
                for(var idx = 0; idx < columns.Count - 2; ++idx)
                {
                    archive.Add(null);
                }

                foreach (var key in row.Keys)
                {
                    if (!columns.Where(col => col.FieldName == key).Any())
                    {
                        columns.Add(new ListColumn()
                        {
                            FieldName = key,
                            Header = key,
                        });

                        foreach(var existingRow in resultRows)
                        {
                            existingRow.Add(null);
                        }

                        archive.Add(null);
                    }

                    var keyIndex = columns.FindIndex(col => col.FieldName == key);

                    archive[keyIndex] = row[key];
                }

                resultRows.Add(archive);
            }

            listResponse.Columns = columns;
            listResponse.Model = resultRows;

            return listResponse;
        }
    }
}
