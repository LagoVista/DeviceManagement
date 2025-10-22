// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7b094fbc51b2dcfaa804cd1b2411e2b7bc187af4e78823ba6d22a5c4483826bc
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LagoVista.IoT.DeviceManagement.Core.Reporting;
using System.Collections.Generic;
using LagoVista.IoT.DeviceManagement.Core.Models;

namespace LagoVista.IoT.DeviceManagement.Core.Tests
{
    [TestClass]
    public class CreateNormalizedDeviceArchiveTests
    {
        private Dictionary<string, object> CreateRow(params KeyValuePair<string, object>[] args)
        {
            var rnd = new Random();

            var row = new Dictionary<string, object>() {
                {nameof(DeviceArchive.RowKey),"whocares" },
                {nameof(DeviceArchive.PartitionKey),"whocares" },
                {"odata.etag","whocares" },
                {nameof(DeviceArchive.DeviceId),"whocares" },
                {nameof(DeviceArchive.DeviceConfigurationVersionId),"whocares" },
                {nameof(DeviceArchive.DeviceConfigurationId),"whocares" },
                {nameof(DeviceArchive.Timestamp),DateTime.UtcNow.AddMinutes(-rnd.NextDouble() * 1000.0) },
                {nameof(DeviceArchive.PEMMessageId),"abc123" },
            };

            foreach(var arg in args)
            {
                row.Add(arg.Key, arg.Value);
            }

            return row;
        }


        [TestMethod]
        public void BuildWithEmpty()
        {
            var utils = new DeviceArchiveReportUtils();

            var dataSet = new List<Dictionary<string, object>>();

            var result = utils.CreateNormalizedDeviceArchiveResult(dataSet);
            Assert.AreEqual(2, result.Columns.Count());
            Assert.AreEqual(result.Columns.ToList()[0].Header, nameof(DeviceArchive.Timestamp));
            Assert.AreEqual(result.Columns.ToList()[0].Header, nameof(DeviceArchive.Timestamp));
            Assert.AreEqual(result.Columns.ToList()[1].Header, nameof(DeviceArchive.PEMMessageId));
            Assert.AreEqual(result.Columns.ToList()[1].Header, nameof(DeviceArchive.PEMMessageId));

            Assert.AreEqual(0, result.Model.Count());
        }


        [TestMethod]
        public void BuildWithOneRow1Columns()
        {
            var utils = new DeviceArchiveReportUtils();

            var dataSet = new List<Dictionary<string, object>>();

            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row1One")));

            var result = utils.CreateNormalizedDeviceArchiveResult(dataSet);
            Assert.AreEqual(2 + 1, result.Columns.Count());
            Assert.AreEqual(result.Columns.ToList()[0].Header, nameof(DeviceArchive.Timestamp));
            Assert.AreEqual(result.Columns.ToList()[0].Header, nameof(DeviceArchive.Timestamp));
            Assert.AreEqual(result.Columns.ToList()[1].Header, nameof(DeviceArchive.PEMMessageId));
            Assert.AreEqual(result.Columns.ToList()[1].Header, nameof(DeviceArchive.PEMMessageId));


            Assert.AreEqual(result.Columns.ToList()[2].FieldName, "One");
            Assert.AreEqual(result.Columns.ToList()[2].Header, "One");

            Assert.AreEqual(result.Model.ToList()[0].Count(), 3);
            Assert.AreEqual(1, result.Model.Count());

            Assert.AreEqual("Row1One", result.Model.ToList()[0][2]);
        }

        [TestMethod]
        public void BuildWithOneRow3Columns()
        {
            var utils = new DeviceArchiveReportUtils();

            var dataSet = new List<Dictionary<string, object>>();

            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row1One"),new KeyValuePair<string, object>("Two", "Row1Two"), new KeyValuePair<string, object>("Three","Row1Three")));

            var result = utils.CreateNormalizedDeviceArchiveResult(dataSet);
            Assert.AreEqual(2 + 3, result.Columns.Count());
            Assert.AreEqual(result.Columns.ToList()[0].Header, nameof(DeviceArchive.Timestamp));
            Assert.AreEqual(result.Columns.ToList()[0].Header, nameof(DeviceArchive.Timestamp));
            Assert.AreEqual(result.Columns.ToList()[1].Header, nameof(DeviceArchive.PEMMessageId));
            Assert.AreEqual(result.Columns.ToList()[1].Header, nameof(DeviceArchive.PEMMessageId));

                    
            Assert.AreEqual(result.Columns.ToList()[2].FieldName, "One");
            Assert.AreEqual(result.Columns.ToList()[2].Header, "One");
            Assert.AreEqual(result.Columns.ToList()[3].FieldName, "Two");
            Assert.AreEqual(result.Columns.ToList()[3].Header, "Two");
            Assert.AreEqual(result.Columns.ToList()[4].FieldName, "Three");
            Assert.AreEqual(result.Columns.ToList()[4].Header, "Three");

            Assert.AreEqual(result.Model.ToList()[0].Count(), 5);
            Assert.AreEqual(1, result.Model.Count());

            Assert.AreEqual("Row1One", result.Model.ToList()[0][2]);
            Assert.AreEqual("Row1Two", result.Model.ToList()[0][3]);
            Assert.AreEqual("Row1Three", result.Model.ToList()[0][4]);
        }

        [TestMethod]
        public void BuildWithTwoRows3Columns()
        {
            var utils = new DeviceArchiveReportUtils();
            var dataSet = new List<Dictionary<string, object>>();

            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row1One"), new KeyValuePair<string, object>("Two", "Row1Two"), new KeyValuePair<string, object>("Three", "Row1Three")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row2One"), new KeyValuePair<string, object>("Two", "Row2Two"), new KeyValuePair<string, object>("Three", "Row2Three")));

            var result = utils.CreateNormalizedDeviceArchiveResult(dataSet);
            Assert.AreEqual(2, result.Model.Count());
            Assert.AreEqual(2 + 3, result.Columns.Count());

            Assert.AreEqual(result.Model.ToList()[0].Count(), 5);
            Assert.AreEqual("Row1One", result.Model.ToList()[0][2]);
            Assert.AreEqual("Row1Two", result.Model.ToList()[0][3]);
            Assert.AreEqual("Row1Three", result.Model.ToList()[0][4]);

            Assert.AreEqual(result.Model.ToList()[1].Count(), 5);
            Assert.AreEqual("Row2One", result.Model.ToList()[1][2]);
            Assert.AreEqual("Row2Two", result.Model.ToList()[1][3]);
            Assert.AreEqual("Row2Three", result.Model.ToList()[1][4]);
        }

        [TestMethod]
        public void BuildWithThreeRows5Columns_Uneven()
        {
            var utils = new DeviceArchiveReportUtils();
            var dataSet = new List<Dictionary<string, object>>();            

            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row1One"), new KeyValuePair<string, object>("Two", "Row1Two"), new KeyValuePair<string, object>("Three", "Row1Three")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row2One"), new KeyValuePair<string, object>("Two", "Row2Two"), new KeyValuePair<string, object>("Three", "Row2Three")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("Two", "Row3Two"), new KeyValuePair<string, object>("Three", "Row3Three"), new KeyValuePair<string, object>("Four", "Row3Four")));

            var result = utils.CreateNormalizedDeviceArchiveResult(dataSet);
            Assert.AreEqual(3, result.Model.Count());
            Assert.AreEqual(2 + 4, result.Columns.Count());

            Assert.AreEqual(result.Columns.ToList()[2].FieldName, "One");
            Assert.AreEqual(result.Columns.ToList()[2].Header, "One");
            Assert.AreEqual(result.Columns.ToList()[3].FieldName, "Two");
            Assert.AreEqual(result.Columns.ToList()[3].Header, "Two");
            Assert.AreEqual(result.Columns.ToList()[4].FieldName, "Three");
            Assert.AreEqual(result.Columns.ToList()[5].Header, "Four");
            Assert.AreEqual(result.Columns.ToList()[5].Header, "Four");

            Assert.AreEqual("Row1One", result.Model.ToList()[0][2]);
            Assert.AreEqual("Row1Two", result.Model.ToList()[0][3]);
            Assert.AreEqual("Row1Three", result.Model.ToList()[0][4]);
            Assert.IsNull(result.Model.ToList()[0][5]);

            Assert.AreEqual("Row2One", result.Model.ToList()[1][2]);
            Assert.AreEqual("Row2Two", result.Model.ToList()[1][3]);
            Assert.AreEqual("Row2Three", result.Model.ToList()[1][4]);
            Assert.IsNull(result.Model.ToList()[1][5]);

            Assert.IsNull(result.Model.ToList()[2][2]);
            Assert.AreEqual("Row3Two", result.Model.ToList()[2][3]);
            Assert.AreEqual("Row3Three", result.Model.ToList()[2][4]);
            Assert.AreEqual("Row3Four", result.Model.ToList()[2][5]);
        }

        [TestMethod]
        public void BuildWithFourRows5Columns_Uneven()
        {
            var utils = new DeviceArchiveReportUtils();
            var dataSet = new List<Dictionary<string, object>>();

            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row1One"), new KeyValuePair<string, object>("Two", "Row1Two"), new KeyValuePair<string, object>("Three", "Row1Three")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row2One"), new KeyValuePair<string, object>("Two", "Row2Two"), new KeyValuePair<string, object>("Three", "Row2Three")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("Two", "Row3Two"), new KeyValuePair<string, object>("Three", "Row3Three"), new KeyValuePair<string, object>("Four", "Row3Four")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row4One"), new KeyValuePair<string, object>("Three", "Row4Three"), new KeyValuePair<string, object>("Four", "Row4Four")));

            var result = utils.CreateNormalizedDeviceArchiveResult(dataSet);
            Assert.AreEqual(4, result.Model.Count());
            Assert.AreEqual(2 + 4, result.Columns.Count());

            Assert.AreEqual(result.Columns.ToList()[2].FieldName, "One");
            Assert.AreEqual(result.Columns.ToList()[2].Header, "One");
            Assert.AreEqual(result.Columns.ToList()[3].FieldName, "Two");
            Assert.AreEqual(result.Columns.ToList()[3].Header, "Two");
            Assert.AreEqual(result.Columns.ToList()[4].FieldName, "Three");
            Assert.AreEqual(result.Columns.ToList()[5].Header, "Four");
            Assert.AreEqual(result.Columns.ToList()[5].Header, "Four");

            Assert.AreEqual("Row1One", result.Model.ToList()[0][2]);
            Assert.AreEqual("Row1Two", result.Model.ToList()[0][3]);
            Assert.AreEqual("Row1Three", result.Model.ToList()[0][4]);
            Assert.IsNull(result.Model.ToList()[0][5]);

            Assert.AreEqual("Row2One", result.Model.ToList()[1][2]);
            Assert.AreEqual("Row2Two", result.Model.ToList()[1][3]);
            Assert.AreEqual("Row2Three", result.Model.ToList()[1][4]);
            Assert.IsNull(result.Model.ToList()[1][5]);

            Assert.IsNull(result.Model.ToList()[2][2]);
            Assert.AreEqual("Row3Two", result.Model.ToList()[2][3]);
            Assert.AreEqual("Row3Three", result.Model.ToList()[2][4]);
            Assert.AreEqual("Row3Four", result.Model.ToList()[2][5]);
            
            Assert.AreEqual("Row4One", result.Model.ToList()[3][2]);
            Assert.IsNull(result.Model.ToList()[3][3]);
            Assert.AreEqual("Row4Three", result.Model.ToList()[3][4]);
            Assert.AreEqual("Row4Four", result.Model.ToList()[3][5]);
        }

        [TestMethod]
        public void BuildWithFourRows6Columns_Uneven()
        {
            var utils = new DeviceArchiveReportUtils();
            var dataSet = new List<Dictionary<string, object>>();

            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row1One"), new KeyValuePair<string, object>("Two", "Row1Two"), new KeyValuePair<string, object>("Three", "Row1Three")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row2One"), new KeyValuePair<string, object>("Two", "Row2Two"), new KeyValuePair<string, object>("Three", "Row2Three")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("Two", "Row3Two"), new KeyValuePair<string, object>("Three", "Row3Three"), new KeyValuePair<string, object>("Four", "Row3Four")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("One", "Row4One"), new KeyValuePair<string, object>("Three", "Row4Three"), new KeyValuePair<string, object>("Four", "Row4Four")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("Five", "OneAtEnd")));
            dataSet.Add(CreateRow(new KeyValuePair<string, object>("Three", "LastOne")));

            var result = utils.CreateNormalizedDeviceArchiveResult(dataSet);
            Assert.AreEqual(6, result.Model.Count());
            Assert.AreEqual(2 + 5, result.Columns.Count());

            Assert.AreEqual(result.Columns.ToList()[2].FieldName, "One");
            Assert.AreEqual(result.Columns.ToList()[2].Header, "One");
            Assert.AreEqual(result.Columns.ToList()[3].FieldName, "Two");
            Assert.AreEqual(result.Columns.ToList()[3].Header, "Two");
            Assert.AreEqual(result.Columns.ToList()[4].FieldName, "Three");
            Assert.AreEqual(result.Columns.ToList()[5].Header, "Four");
            Assert.AreEqual(result.Columns.ToList()[5].Header, "Four");
            Assert.AreEqual(result.Columns.ToList()[6].Header, "Five");
            Assert.AreEqual(result.Columns.ToList()[6].Header, "Five");

            foreach(var row in result.Model)
            {
                Assert.AreEqual(result.Columns.Count(), row.Count());
            }

            Assert.AreEqual("Row1One", result.Model.ToList()[0][2]);
            Assert.AreEqual("Row1Two", result.Model.ToList()[0][3]);
            Assert.AreEqual("Row1Three", result.Model.ToList()[0][4]);
            Assert.IsNull(result.Model.ToList()[0][5]);
            Assert.IsNull(result.Model.ToList()[0][6]);

            Assert.AreEqual("Row2One", result.Model.ToList()[1][2]);
            Assert.AreEqual("Row2Two", result.Model.ToList()[1][3]);
            Assert.AreEqual("Row2Three", result.Model.ToList()[1][4]);
            Assert.IsNull(result.Model.ToList()[1][5]);
            Assert.IsNull(result.Model.ToList()[1][6]);

            Assert.IsNull(result.Model.ToList()[2][2]);
            Assert.AreEqual("Row3Two", result.Model.ToList()[2][3]);
            Assert.AreEqual("Row3Three", result.Model.ToList()[2][4]);
            Assert.AreEqual("Row3Four", result.Model.ToList()[2][5]);
            Assert.IsNull(result.Model.ToList()[2][6]);

            Assert.AreEqual("Row4One", result.Model.ToList()[3][2]);
            Assert.IsNull(result.Model.ToList()[3][3]);
            Assert.AreEqual("Row4Three", result.Model.ToList()[3][4]);
            Assert.AreEqual("Row4Four", result.Model.ToList()[3][5]);
            Assert.IsNull(result.Model.ToList()[3][6]);

            Assert.IsNull(result.Model.ToList()[4][2]);
            Assert.IsNull(result.Model.ToList()[4][3]);
            Assert.IsNull(result.Model.ToList()[4][4]);
            Assert.IsNull(result.Model.ToList()[4][5]);
            Assert.AreEqual("OneAtEnd", result.Model.ToList()[4][6]);
            
            Assert.IsNull(result.Model.ToList()[5][2]);
            Assert.IsNull(result.Model.ToList()[5][3]);
            Assert.AreEqual("LastOne", result.Model.ToList()[5][4]);
            Assert.IsNull(result.Model.ToList()[5][5]);
            Assert.IsNull(result.Model.ToList()[5][6]);
        }
    }
}
