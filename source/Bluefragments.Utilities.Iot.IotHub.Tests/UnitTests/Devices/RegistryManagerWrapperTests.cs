using System;
using System.Collections.Generic;
using System.Text;
using Dpx.Iot.Infrastructure.Devices;
using Xunit;

namespace Dpx.Iot.Infrastructure.Tests.UnitTests.Devices
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
