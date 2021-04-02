using System;
using System.Collections.Generic;
using System.Text;
using Bluefragments.Utilities.Extensions;
using Bluefragments.Utilities.Iot.IotHub.Entities;

namespace Bluefragments.Utilities.Iot.IotHub.Extensions
{
    public static class DeviceExtentions
    {
        private const string ConnectionStringTemplate = "HostName={0};DeviceId={1};SharedAccessKey={2}";

        public static string GetDeviceConnectionString(this Device device)
        {
            device.ThrowIfParameterIsNull(nameof(device));

            device.HostName.ThrowIfParameterIsNullOrWhiteSpace(nameof(device.HostName));
            device.Id.ThrowIfParameterIsNullOrWhiteSpace(nameof(device.Id));
            device.PrimaryKey.ThrowIfParameterIsNullOrWhiteSpace(nameof(device.PrimaryKey));

            return string.Format(ConnectionStringTemplate, device.HostName, device.Id, device.PrimaryKey);
        }
    }
}
