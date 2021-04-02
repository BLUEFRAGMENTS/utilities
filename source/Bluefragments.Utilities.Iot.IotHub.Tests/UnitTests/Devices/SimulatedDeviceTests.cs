using System;
using Bluefragments.Utilities.Iot.IotHub.Devices;
using Xunit;

namespace Bluefragments.Utilities.Iot.IotHub.Tests.UnitTests.Devices
{
    public class SimulatedDeviceTests
    {
        [Fact]
        public void Ctor_NullDeviceClient_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SimulatedDevice(null!));
        }
    }
}
