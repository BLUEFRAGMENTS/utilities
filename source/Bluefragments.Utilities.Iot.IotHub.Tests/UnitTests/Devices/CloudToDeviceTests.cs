using System;
using System.Collections.Generic;
using System.Text;
using Dpx.Iot.Infrastructure.Devices;
using Xunit;

namespace Dpx.Iot.Infrastructure.Tests.UnitTests.Devices
{
    public class CloudToDeviceTests
    {
        [Fact]
        public void Ctor_EmptyConnectionString_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CloudToDevice(string.Empty));
        }
    }
}
