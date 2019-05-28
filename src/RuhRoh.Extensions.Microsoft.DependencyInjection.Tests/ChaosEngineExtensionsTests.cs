using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using RuhRoh.Extensions.Microsoft.DependencyInjection.Tests.Services;
using Xunit;

namespace RuhRoh.Extensions.Microsoft.DependencyInjection.Tests
{
    public class ChaosEngineExtensionsTests : IDisposable
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        [Theory]
        [InlineData(ServiceLifetime.Scoped)]
        [InlineData(ServiceLifetime.Singleton)]
        [InlineData(ServiceLifetime.Transient)]
        public void Affect_Replaces_Service_And_Adds_Implementation(ServiceLifetime lifetime)
        {
            // Given
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    _services.AddScoped<ITestServiceContract, TestService>();
                    break;
                case ServiceLifetime.Singleton:
                    _services.AddSingleton<ITestServiceContract, TestService>();
                    break;
                case ServiceLifetime.Transient:
                    _services.AddTransient<ITestServiceContract, TestService>();
                    break;
            }

            // When
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    _services.AffectScoped<ITestServiceContract, TestService>();
                    break;
                case ServiceLifetime.Singleton:
                    _services.AffectSingleton<ITestServiceContract, TestService>();
                    break;
                case ServiceLifetime.Transient:
                    _services.AffectTransient<ITestServiceContract, TestService>();
                    break;
            }

            // Then
            Assert.Equal(2, _services.Count);

            var firstServiceDescriptor = _services.ElementAt(0);
            Assert.Equal(typeof(ITestServiceContract), firstServiceDescriptor.ServiceType);
            Assert.NotNull(firstServiceDescriptor.ImplementationFactory);
            Assert.Null(firstServiceDescriptor.ImplementationType);
            Assert.Null(firstServiceDescriptor.ImplementationInstance);
            Assert.Equal(lifetime, firstServiceDescriptor.Lifetime);

            var secondServiceDescriptor = _services.ElementAt(1);
            Assert.Equal(typeof(TestService), secondServiceDescriptor.ServiceType);
            Assert.Null(secondServiceDescriptor.ImplementationFactory);
            Assert.Equal(typeof(TestService), secondServiceDescriptor.ImplementationType);
            Assert.Null(firstServiceDescriptor.ImplementationInstance);
            Assert.Equal(lifetime, secondServiceDescriptor.Lifetime);

            var affectedServices = ChaosEngineExtensions.GetAffectedServices();
            Assert.Equal(1, affectedServices.Count);

            var affectedService = affectedServices.First();
            Assert.Equal(typeof(ITestServiceContract), affectedService.Key);
            Assert.IsType<AffectedService<ITestServiceContract, TestService>>(affectedService.Value);
        }

        [Fact]
        public void Affecting_The_Same_Service_Multiple_Times_Works()
        {
            // Given
            _services.AddSingleton<ITestServiceContract, TestService>();

            // When
            _services.AffectSingleton<ITestServiceContract, TestService>()
                .WhenCalling(x => x.GetItemById(With.Any<int>()))
                .SlowItDownBy(TimeSpan.FromSeconds(5))
                .AtRandom();

            _services.AffectSingleton<ITestServiceContract, TestService>()
                .WhenCalling(x => x.GetItems())
                .Throw<Exception>()
                .AtRandom();

            // Then
            Assert.Equal(2, _services.Count);

            var firstServiceDescriptor = _services.ElementAt(0);
            Assert.Equal(typeof(ITestServiceContract), firstServiceDescriptor.ServiceType);
            Assert.NotNull(firstServiceDescriptor.ImplementationFactory);
            Assert.Null(firstServiceDescriptor.ImplementationType);
            Assert.Null(firstServiceDescriptor.ImplementationInstance);

            var secondServiceDescriptor = _services.ElementAt(1);
            Assert.Equal(typeof(TestService), secondServiceDescriptor.ServiceType);
            Assert.Null(secondServiceDescriptor.ImplementationFactory);
            Assert.Equal(typeof(TestService), secondServiceDescriptor.ImplementationType);
            Assert.Null(firstServiceDescriptor.ImplementationInstance);

            var affectedServices = ChaosEngineExtensions.GetAffectedServices();
            Assert.Equal(1, affectedServices.Count);

            var affectedService = affectedServices.First();
            Assert.Equal(typeof(ITestServiceContract), affectedService.Key);
            Assert.IsType<AffectedService<ITestServiceContract, TestService>>(affectedService.Value);
        }

        public void Dispose()
        {
            ChaosEngineExtensions.ResetAffectedServices();
        }
    }
}
