using System;
using Dpx.Iot.Infrastructure.Devices;
using Xunit;

namespace Dpx.Iot.Infrastructure.Tests.UnitTests.Devices
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
