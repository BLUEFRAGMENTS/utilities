using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bluefragments.Utilities.Extensions;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace Bluefragments.Utilities.Iot.IotHub.Devices
{
    public class SimulatedDevice : IDisposable
    {
        private readonly DeviceClient deviceClient;
        private bool isDisposed;

        internal SimulatedDevice(DeviceClient deviceClient)
        {
            deviceClient.ThrowIfParameterIsNull(nameof(deviceClient));
            this.deviceClient = deviceClient;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task SendDataPointAsync<T>(T dataPoint) => SendDataPointAsync(dataPoint, new Dictionary<string, string>());

        public async Task SendDataPointAsync<T>(T dataPoint, IDictionary<string, string> properties)
        {
            dataPoint.ThrowIfParameterIsNull(nameof(dataPoint));
            properties.ThrowIfParameterIsNull(nameof(properties));

            var jsonString = JsonConvert.SerializeObject(dataPoint);
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);
            var message = new Message(messageBytes);

            foreach (var keyValuePair in properties)
            {
                message.Properties.Add(keyValuePair);
            }

            await deviceClient.SendEventAsync(message);
        }

        public async Task<T?> RecieveMessageAsync<T>()
            where T : class
        {
            var recievedMessage = await deviceClient.ReceiveAsync();
            if (recievedMessage is null)
            {
                return default;
            }

            await deviceClient.CompleteAsync(recievedMessage);
            var json = Encoding.UTF8.GetString(recievedMessage!.GetBytes());
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            deviceClient?.Dispose();
            isDisposed = true;
        }
    }
}
