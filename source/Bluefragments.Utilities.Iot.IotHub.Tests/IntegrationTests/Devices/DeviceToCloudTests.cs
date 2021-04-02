using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bluefragments.Iot.Devices;
using Bluefragments.Utilities.Iot.IotHub.Devices;
using Xunit;
using Xunit.Sdk;

namespace Bluefragments.Utilities.Iot.IotHub.Tests.IntegrationTests.Devices
{
    [CollectionDefinition(nameof(CloudToDeviceTests), DisableParallelization = true)]
    public class DeviceToCloudTests : TestBase, IAsyncLifetime
    {
        private const string DeviceId = "SimulationDevice";

        private SimulatedDevice simulatedDevice = default!;
        private Registration registration = default!;

        public async Task InitializeAsync()
        {
            var connectionString = Configuration["IotHubConnectionString"];
            registration = new Registration(new RegistryManagerWrapper(connectionString));

            await registration.RemoveDeviceAsync(DeviceId);
            var device = await registration.RegisterDeviceAsync(DeviceId);

            var deviceToCloud = new DeviceToCloud();
            if (device is null)
            {
                throw new XunitException("Device does not exist");
            }

            simulatedDevice = deviceToCloud.CreateSimulatedDevice(device);
        }
       
        public async Task DisposeAsync()
        {
            await registration.RemoveDeviceAsync(DeviceId);
            registration.Dispose();
            simulatedDevice.Dispose();
        }

        [Fact]
        public async Task SendDataPointAsync_Succeeds()
        {
            var data = new { hello = "World" };
            await simulatedDevice.SendDataPointAsync(data);
        }

        [Fact]
        public async Task SendDataPointAsync_WithProperties_Succeeds()
        {
            var data = new { hello = "World" };

            var properties = new Dictionary<string, string>();
            properties.Add("Hello", "World");

            await simulatedDevice.SendDataPointAsync(data, properties);
        }
    }
}
