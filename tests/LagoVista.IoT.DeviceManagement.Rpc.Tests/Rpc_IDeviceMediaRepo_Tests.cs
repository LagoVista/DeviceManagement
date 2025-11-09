// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 04dd500987b54cb07e462a0e0df54f6096498cfec8ae107a1b5dbef139e1ae9b
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.IoT.DeviceManagement.Rpc.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.DeviceManagement.Rpc.Tests
{
    [TestClass]
    public class Rpc_IDeviceMediaRepo_Tests
    {
        [ClassInitialize]
        static public void ClassInitialize(TestContext testContext)
        {
            DataFactory.Initialize();
        }

        //private readonly static MethodInfo[] _objectMethods = typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.Public);

        //public int AddService<TInterface>(TInterface subject) where TInterface : class
        //{
        //    if (subject == null)
        //    {
        //        throw new ArgumentNullException(nameof(subject));
        //    }

        //    var interfaceType = typeof(TInterface);
        //    if (!interfaceType.IsInterface)
        //    {
        //        throw new ArgumentException($"TInterface type must be an interface: '{interfaceType.FullName}'.");
        //    }

        //    var methods = interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
        //        .Except(_objectMethods)
        //        .Where(m => m.GetCustomAttribute<RpcIgnoreMethodAttribute>() == null);
        //    var methodCount = 0;
        //    foreach (var method in methods)
        //    {
        //        //RegisterSubjectMethod(subject, method);
        //        ++methodCount;
        //    }

        //    var interfaces = interfaceType.GetInterfaces();
        //    foreach (var item in interfaces)
        //    {
        //        methodCount += AddService(subject);
        //    }

        //    return methodCount;
        //}

        //[TestMethod]
        //public void TestMethodCount()
        //{
        //    var count = AddService(DataFactory.DeviceMediaRepo);
        //}

        [TestMethod]
        public async Task AddMediaAsync()
        {
            //var json = Properties.Resources.jsonbug;
            //var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            //var jsonbytes = Encoding.UTF8.GetBytes(json);
            //var message = new Message(jsonbytes);

            var value = "Hello, world.";
            var bytes = Encoding.UTF8.GetBytes(value);
            var fileName = Guid.NewGuid().ToId();
            var contentType = "text";

            var addResponse = await DataFactory.DeviceMediaRepoRemote.AddMediaAsync(DataFactory.DeviceRepo, bytes, fileName, contentType);
            Assert.IsTrue(addResponse.Successful);
        }

        [TestMethod]
        public async Task DeleteMediaAsync()
        {
            var value = "Hello, world.";
            var bytes = Encoding.UTF8.GetBytes(value);
            var fileName = Guid.NewGuid().ToId();
            var contentType = "text";

            var addResponse = await DataFactory.DeviceMediaRepo.AddMediaAsync(DataFactory.DeviceRepo, bytes, fileName, contentType);
            Assert.IsTrue(addResponse.Successful);

            var deleteResponse = await DataFactory.DeviceMediaRepo.DeleteMediaAsync(DataFactory.DeviceRepo, fileName);
            Assert.IsTrue(deleteResponse.Successful);
        }

        [TestMethod]
        public async Task GetMediaAsync()
        {
            var value = "Hello, world.";
            var bytes = Encoding.UTF8.GetBytes(value);
            var fileName = Guid.NewGuid().ToId();
            var contentType = "text";

            var addResponse = await DataFactory.DeviceMediaRepo.AddMediaAsync(DataFactory.DeviceRepo, bytes, fileName, contentType);
            Assert.IsTrue(addResponse.Successful);

            var getResponse = await DataFactory.DeviceMediaRepo.GetMediaAsync(DataFactory.DeviceRepo, fileName);
            Assert.IsTrue(getResponse.Successful);
            Assert.IsTrue(bytes.SequenceEqual(getResponse.Result));
        }
    }
}
