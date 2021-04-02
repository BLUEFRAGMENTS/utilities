using System;
using System.Threading.Tasks;
using Bluefragments.Iot.Devices;
using Dpx.Iot.Infrastructure.Devices;
using Xunit;
using Xunit.Sdk;

namespace Dpx.Iot.Infrastructure.Tests.IntegrationTests.Devices
{
    [CollectionDefinition(nameof(CloudToDeviceTests), DisableParallelization = true)]
    public class CloudToDeviceTests : TestBase, IAsyncLifetime
    {
        private const string DeviceId = "TestRegistrationDevice";

        private readonly CloudToDevice cloudToDevice;
        private readonly Registration registration;

        public CloudToDeviceTests()
        {
            var connectionString = Configuration["IotHubConnectionString"];

            cloudToDevice = new CloudToDevice(connectionString);
            registration = new Registration(new RegistryManagerWrapper(connectionString));
        }

        public async Task DisposeAsync()
        {
            await registration.RemoveDeviceAsync(DeviceId);
            registration.Dispose();
            cloudToDevice.Dispose();
        }

        public async Task InitializeAsync()
        {
            await registration.RemoveDeviceAsync(DeviceId);
        }

        [Fact]
        public async Task SendMessageToDevice_SameMessageIsRecieved()
        {
            var device = await registration.RegisterDeviceAsync(DeviceId);

            var deviceToCloud = new DeviceToCloud();
            if (device is null)
            {
                throw new XunitException("Device does not exist");
            }

            using var simulatedDevice = deviceToCloud.CreateSimulatedDevice(device);

            var recieveTask = simulatedDevice.RecieveMessageAsync<TestMessage>();

            var data = new TestMessage { Temperature = 11, Humdity = 40 };
            await cloudToDevice.SendMessageToDeviceAsync(data, device);

            var result = await recieveTask;

            Assert.NotNull(result);
            Assert.Equal(data, result!);
        }

        private class TestMessage : IEquatable<TestMessage>
        {
            public int Temperature { get; set; }

            public int Humdity { get; set; }

            public bool Equals(TestMessage? other)
            {
                return other?.Temperature.Equals(Temperature) == true
                    && other?.Humdity.Equals(Humdity) == true;
            }
        }
    }
}
