using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Dpx.Iot.Infrastructure.Tests.UnitTests.Devices
{
    public class IoTClientTests
    {
        [Fact]
        public void GetCloudToDevice_NotInitialized_ThrowsInvalidOperationException()
        {
            var client = new IotClient();
            Assert.Throws<InvalidOperationException>(() => client.CloudToDevice);
        }

        [Fact]
        public void GetDeviceToCloud_NotInitialized_ThrowsInvalidOperationException()
        {
            var client = new IotClient();
            Assert.Throws<InvalidOperationException>(() => client.DeviceToCloud);
        }

        [Fact]
        public void GetRegistration_NotInitialized_ThrowsInvalidOperationException()
        {
            var client = new IotClient();
            Assert.Throws<InvalidOperationException>(() => client.Registration);
        }
    }
}
