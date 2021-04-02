using System;
using Bluefragments.Iot.Devices;
using Bluefragments.Utilities.Iot.IotHub.Devices;

namespace Bluefragments.Utilities.Iot.IotHub
{
    public interface IIotClient : IDisposable
    {
        IRegistration Registration { get; }

        IDeviceToCloud DeviceToCloud { get; }

        void Initialize(string iotHubConnectionString);
    }
}
