using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core;

namespace LagoVista.IoT.DeviceManagement.Core.Tests.DataStreamTests
{
    [TestClass]
    public class DataStreaamValidaationTests
    {
        private void AssertValidModel(ValidationResult result)
        {
            if(result.Errors.Any())
            {
                Console.WriteLine("Errors where there shouldn't be any");
            }

            foreach(var err in result.Errors)
            {
                Console.WriteLine("   " + err.Message);
            }

            Assert.IsTrue(result.Successful);
        }

        private void AssertInvalidModel(ValidationResult result, params string[] args)
        {
            Console.WriteLine("Found errors (expected):");

            foreach (var err in result.Errors)
            {
                Console.WriteLine("   " + err.Message);
            }

            Assert.AreEqual(result.Errors.Count, args.Length);

            foreach(var arg in args)
            {
                Assert.IsTrue(result.Errors.Where(err => err.Message == arg).Any(), $"Error message [{arg}] missing from errors.");
            }
           

            Assert.IsFalse(result.Successful);
        }

        [TestMethod]
        public void DataStream_Basic_Valid()
        {
            var stream = new DataStream();
            stream.Id = "A8A087E53D2043538F32FB18C2CA67F7";
            stream.Name = "mystream";
            stream.Key = "streamkey";
            stream.CreationDate = DateTime.Now.ToJSONString();
            stream.LastUpdatedDate = DateTime.Now.ToJSONString();
            stream.CreatedBy = EntityHeader.Create("A8A087E53D2043538F32FB18C2CA67F7", "user");
            stream.LastUpdatedBy = stream.CreatedBy;
            stream.StreamType = EntityHeader<DataStreamTypes>.Create(DataStreamTypes.AWSS3);
           
            AssertValidModel(Validator.Validate(stream));
        }

        [TestMethod]
        public void DataStream_WithFields_Valid()
        {
            var stream = new DataStream();
            stream.Id = "A8A087E53D2043538F32FB18C2CA67F7";
            stream.Name = "mystream";
            stream.Key = "streamkey";
            stream.CreationDate = DateTime.Now.ToJSONString();
            stream.LastUpdatedDate = DateTime.Now.ToJSONString();
            stream.CreatedBy = EntityHeader.Create("A8A087E53D2043538F32FB18C2CA67F7", "user");
            stream.LastUpdatedBy = stream.CreatedBy;
            stream.StreamType = EntityHeader<DataStreamTypes>.Create(DataStreamTypes.AWSS3);
            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name1",
                Key = "key1",
                FieldName = "fieldname1",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String)
            });

            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name2",
                Key = "key2",
                FieldName = "fieldname2",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String)
            });

            AssertValidModel(Validator.Validate(stream));
        }

        [TestMethod]
        public void DataStream_WithFields_Invalid_DuplicateKeyOnFields()
        {
            var stream = new DataStream();
            stream.Id = "A8A087E53D2043538F32FB18C2CA67F7";
            stream.Name = "mystream";
            stream.Key = "streamkey";
            stream.CreationDate = DateTime.Now.ToJSONString();
            stream.LastUpdatedDate = DateTime.Now.ToJSONString();
            stream.CreatedBy = EntityHeader.Create("A8A087E53D2043538F32FB18C2CA67F7", "user");
            stream.LastUpdatedBy = stream.CreatedBy;
            stream.StreamType = EntityHeader<DataStreamTypes>.Create(DataStreamTypes.AWSS3);
            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name1",
                Key = "key1",
                FieldName = "fieldname1",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String)
            });

            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name2",
                Key = "key1",
                FieldName = "fieldname2",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String)
            });

            AssertInvalidModel(Validator.Validate(stream), "Keys on fields must be unique");
        }

        [TestMethod]
        public void DataStream_WithFields_Invalid_DuplicateFieldNamesOnFields()
        {
            var stream = new DataStream();
            stream.Id = "A8A087E53D2043538F32FB18C2CA67F7";
            stream.Name = "mystream";
            stream.Key = "streamkey";
            stream.CreationDate = DateTime.Now.ToJSONString();
            stream.LastUpdatedDate = DateTime.Now.ToJSONString();
            stream.CreatedBy = EntityHeader.Create("A8A087E53D2043538F32FB18C2CA67F7", "user");
            stream.LastUpdatedBy = stream.CreatedBy;
            stream.StreamType = EntityHeader<DataStreamTypes>.Create(DataStreamTypes.AWSS3);
            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name1",
                Key = "key1",
                FieldName = "fieldname1",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String)
            });

            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name2",
                Key = "key2",
                FieldName = "fieldname1",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String)
            });

            AssertInvalidModel(Validator.Validate(stream), "Field Names on fields must be unique");
        }

        [TestMethod]
        public void DataStream_WithFields_Invalid_MissingStateSet()
        {
            var stream = new DataStream();
            stream.Id = "A8A087E53D2043538F32FB18C2CA67F7";
            stream.Name = "mystream";
            stream.Key = "streamkey";
            stream.CreationDate = DateTime.Now.ToJSONString();
            stream.LastUpdatedDate = DateTime.Now.ToJSONString();
            stream.CreatedBy = EntityHeader.Create("A8A087E53D2043538F32FB18C2CA67F7", "user");
            stream.LastUpdatedBy = stream.CreatedBy;
            stream.StreamType = EntityHeader<DataStreamTypes>.Create(DataStreamTypes.AWSS3);
            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name1",
                Key = "key1",
                FieldName = "fieldname1",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.State)
            });

            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name2",
                Key = "key2",
                FieldName = "fieldname2",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String)
            });

            AssertInvalidModel(Validator.Validate(stream), "State Set is required on field Name1");
        }

        [TestMethod]
        public void DataStream_WithFields_Invalid_MissingUnitSetSet()
        {
            var stream = new DataStream();
            stream.Id = "A8A087E53D2043538F32FB18C2CA67F7";
            stream.Name = "mystream";
            stream.Key = "streamkey";
            stream.CreationDate = DateTime.Now.ToJSONString();
            stream.LastUpdatedDate = DateTime.Now.ToJSONString();
            stream.CreatedBy = EntityHeader.Create("A8A087E53D2043538F32FB18C2CA67F7", "user");
            stream.LastUpdatedBy = stream.CreatedBy;
            stream.StreamType = EntityHeader<DataStreamTypes>.Create(DataStreamTypes.AWSS3);
            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name1",
                Key = "key1",
                FieldName = "fieldname1",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.ValueWithUnit)
            });

            stream.Fields.Add(new DataStreamField()
            {
                Name = "Name2",
                Key = "key2",
                FieldName = "fieldname2",
                FieldType = EntityHeader<DeviceAdmin.Models.ParameterTypes>.Create(DeviceAdmin.Models.ParameterTypes.String)
            });

            AssertInvalidModel(Validator.Validate(stream), "Unit Set is required on field Name1");
        }
    }
}
