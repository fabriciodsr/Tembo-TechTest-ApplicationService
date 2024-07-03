using System;
using Xunit;
using FluentAssertions;
using Services.Common.Abstractions.Model;
using Services.Applications.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Applications.Abstractions;
using Services.Applications.Handlers;
using Services.Common.Abstractions.Abstractions;
using Microsoft.Extensions.Options;
using Services.Applications.Tests.Helpers;

namespace Services.Applications.Tests.UnitTests.Validator;

public class ApplicationValidatorTests
{
    private readonly IApplicationValidator _validator;
    private readonly IServiceProvider _serviceProvider;

    public ApplicationValidatorTests()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
        _validator = _serviceProvider.GetRequiredService<IApplicationValidator>();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        services.Configure<EligibilitySettings>(configuration.GetSection("ProductEligibility"));
        services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EligibilitySettings>>().Value);
        services.AddSingleton<IApplicationValidator, ApplicationValidator>();
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 15, 5.00)]
    [InlineData(ProductCode.ProductTwo, 16, 5.00)]
    public void ApplicationValidator_ShouldReturnFalse_WhenAgeIsBelowMinimum(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        // Act
        var result = _validator.ValidateApplication(application, out string failureReason);

        // Assert
        result.Should().BeFalse();
        failureReason.Should().Be("Applicant age does not meet the product's eligibility criteria.");
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 40, 5.00)]
    [InlineData(ProductCode.ProductTwo, 51, 5.00)]
    public void ApplicationValidator_ShouldReturnFalse_WhenAgeIsAboveMaximum(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        // Act
        var result = _validator.ValidateApplication(application, out string failureReason);

        // Assert
        result.Should().BeFalse();
        failureReason.Should().Be("Applicant age does not meet the product's eligibility criteria.");
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 20, 0.30)]
    [InlineData(ProductCode.ProductTwo, 20, 0.20)]
    public void ApplicationValidator_ShouldReturnFalse_WhenPaymentIsBelowMinimum(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        // Act
        var result = _validator.ValidateApplication(application, out string failureReason);

        // Assert
        result.Should().BeFalse();
        failureReason.Should().Be("Payment does not meet the minimum requirement.");
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 25, 2.00)]
    [InlineData(ProductCode.ProductTwo, 25, 2.00)]
    public void ApplicationValidator_ShouldReturnTrue_WhenAgeAndPaymentAreValid(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        // Act
        var result = _validator.ValidateApplication(application, out string failureReason);

        // Assert
        result.Should().BeTrue();
        failureReason.Should().BeNullOrEmpty();
    }
}
