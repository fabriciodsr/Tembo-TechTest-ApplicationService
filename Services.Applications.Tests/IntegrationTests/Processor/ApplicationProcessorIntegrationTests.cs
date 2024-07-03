using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Services.Applications.Abstractions;
using Services.Common.Abstractions.Model;
using Services.Common.Abstractions.Abstractions;
using Services.Applications.Handlers;
using Services.Applications.Model;
using Services.Applications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.Applications.Tests.Helpers;
using Services.Applications.MappingProfiles;

namespace Services.Applications.Tests.IntegrationTests.Processor;

public class ApplicationProcessorIntegrationTests
{
    private readonly ServiceProvider _serviceProvider;

    public ApplicationProcessorIntegrationTests()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        services.Configure<EligibilitySettings>(configuration.GetSection("ProductEligibility"));
        services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EligibilitySettings>>().Value);

        services.AddAutoMapper(typeof(ApplicationMappingProfile));

        services.AddSingleton<AdministratorOne.Abstractions.IAdministrationService, AdministratorOneAdapter>();
        services.AddSingleton<AdministratorTwo.Abstractions.IAdministrationService, AdministratorTwoAdapter>();

        services.AddSingleton<AdministratorOneHandler>();
        services.AddSingleton<AdministratorTwoHandler>();

        services.AddSingleton<IAdministrationServiceHandlerFactory, AdministrationServiceHandlerFactory>();

        services.AddSingleton<IApplicationValidator, ApplicationValidator>();
        services.AddSingleton<IApplicationProcessor, ApplicationProcessor>();
        services.AddSingleton<IKycService, KycService>();
        services.AddSingleton<IBus, InMemoryBus>();
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 20, 150.00)]
    [InlineData(ProductCode.ProductTwo, 25, 310.00)]
    public async Task Process_ShouldCompleteSuccessfully(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var applicationProcessor = _serviceProvider.GetRequiredService<IApplicationProcessor>();
        var bus = _serviceProvider.GetRequiredService<IBus>() as InMemoryBus;

        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        // Act
        await applicationProcessor.Process(application);

        // Assert
        Assert.Contains(bus.Events, e => e is InvestorCreated);
        Assert.Contains(bus.Events, e => e is AccountCreated);
        Assert.Contains(bus.Events, e => e is ApplicationCompleted);
    }
}
