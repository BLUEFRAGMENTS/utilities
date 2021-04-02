using System;
using System.Collections.Generic;
using System.Text;
using Bluefragments.Utilities.Iot.IotHub.Devices;
using Xunit;

namespace Bluefragments.Utilities.Iot.IotHub.Tests.UnitTests.Devices
{
    public class RegistryManagerWrapperTests
    {
        [Fact]
        public void Constructor_NoConnectionString_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RegistryManagerWrapper(string.Empty));
        }
    }
}
