using System;
using System.Collections.Generic;
using System.Text;
using Bluefragments.Utilities.Iot.IotHub.Devices;
using Xunit;

namespace Bluefragments.Utilities.Iot.IotHub.Tests.UnitTests.Devices
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
