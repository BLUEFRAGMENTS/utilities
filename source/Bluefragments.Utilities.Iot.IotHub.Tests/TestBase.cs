using System;
using Microsoft.Extensions.Configuration;

namespace Dpx.Iot.Infrastructure.Tests
{
    public class TestBase : IDisposable
    {
        public TestBase()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<TestBase>();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
        }
    }
}
