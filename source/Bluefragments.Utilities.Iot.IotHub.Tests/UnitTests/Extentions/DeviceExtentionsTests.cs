using System;
using System.Collections.Generic;
using System.Text;
using Dpx.Iot.Infrastructure.Entities;
using Dpx.Iot.Infrastructure.Extensions;
using Microsoft.Azure.Devices;
using Xunit;

namespace Dpx.Iot.Infrastructure.Tests.UnitTests.Extentions
{
    public class DeviceExtentionsTests
    {
        private const string ValidDeviceId = "1";
        private const string ValidHostname = "localhost";
        private const string ValidPrimaryKey = "abc";

        private const string ValidDeviceConnectionString = "HostName=" + ValidHostname + ";DeviceId=" + ValidDeviceId + ";SharedAccessKey=" + ValidPrimaryKey;

        [Fact]
        public void GetDeviceConnectionString_ValidValues_ReturnsExpectedResult()
        {
            var dev = new Entities.Device();
            dev.Id = ValidDeviceId;
            dev.PrimaryKey = ValidPrimaryKey;
            dev.HostName = ValidHostname;

            string result = dev.GetDeviceConnectionString();
            Assert.Equal(result, ValidDeviceConnectionString);
        }

        [Fact]
        public void GetDeviceConnectionString_EmptyHostName_ThrowsArgumentOutOfRangeException()
        {
            var dev = new Entities.Device();
            dev.Id = ValidDeviceId;
            dev.PrimaryKey = ValidPrimaryKey;

            Assert.Throws<ArgumentOutOfRangeException>(() => dev.GetDeviceConnectionString());
        }

        [Fact]
        public void GetDeviceConnectionString_EmptyId_ThrowsArgumentOutOfRangeException()
        {
            var dev = new Entities.Device();
            dev.PrimaryKey = ValidPrimaryKey;
            dev.HostName = ValidHostname;

            Assert.Throws<ArgumentOutOfRangeException>(() => dev.GetDeviceConnectionString());
        }

        [Fact]
        public void GetDeviceConnectionString_EmptyPrimaryKey_ThrowsArgumentOutOfRangeException()
        {
            var dev = new Entities.Device();
            dev.Id = ValidDeviceId;
            dev.HostName = ValidHostname;

            Assert.Throws<ArgumentOutOfRangeException>(() => dev.GetDeviceConnectionString());
        }

        [Fact]
        public void GetDeviceConnectionString_NullDevice_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DeviceExtentions.GetDeviceConnectionString(null!));
        }
    }
}
