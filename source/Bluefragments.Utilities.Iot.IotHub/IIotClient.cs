using System;
using Bluefragments.Iot.Devices;
using Dpx.Iot.Infrastructure.Devices;

namespace Dpx.Iot.Infrastructure
{
    public interface IIotClient : IDisposable
    {
        IRegistration Registration { get; }

        IDeviceToCloud DeviceToCloud { get; }

        void Initialize(string iotHubConnectionString);
    }
}
