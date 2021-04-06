using System;
using System.Runtime.CompilerServices;
using Bluefragments.Iot.Devices;
using Bluefragments.Utilities.Iot.IotHub.Devices;

[assembly: InternalsVisibleTo("Bluefragments.Utilities.Iot.IotHub.Tests")]

namespace Bluefragments.Utilities.Iot.IotHub
{
    public class IotClient : IIotClient, IDisposable
    {
        private IRegistration? registration;
        private IDeviceToCloud? deviceToCloud;
        private ICloudToDevice? cloudToDevice;
        private bool isDisposed;

        public IotClient()
        {
        }

        public IRegistration Registration
        {
            get => registration ?? throw new InvalidOperationException("Uninitialized " + nameof(IotClient));
            private set => registration = value;
        }

        public IDeviceToCloud DeviceToCloud
        {
            get => deviceToCloud ?? throw new InvalidOperationException("Uninitialized " + nameof(IotClient));
            private set => deviceToCloud = value;
        }

        public ICloudToDevice CloudToDevice
        {
            get => cloudToDevice ?? throw new InvalidOperationException("Uninitialized " + nameof(IotClient));
            private set => cloudToDevice = value;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Initialize(string iotHubConnectionString)
        {
            Registration = new Registration(new RegistryManagerWrapper(iotHubConnectionString));
            DeviceToCloud = new DeviceToCloud();
            CloudToDevice = new CloudToDevice(iotHubConnectionString);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            registration?.Dispose();
            cloudToDevice?.Dispose();
            isDisposed = true;
        }
    }
}