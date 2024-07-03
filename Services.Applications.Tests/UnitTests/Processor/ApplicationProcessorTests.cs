using Moq;
using Services.Applications.Abstractions;
using Xunit;
using Services.Common.Abstractions.Model;
using Services.Common.Abstractions.Abstractions;
using Services.Applications.Handlers;
using Services.Applications.Model;
using Services.AdministratorOne.Abstractions.Model;
using Services.Applications.Tests.Helpers;

namespace Services.Applications.Tests.UnitTests.Processor;

public class ApplicationProcessorTests
{
    private readonly Mock<IKycService> _kycServiceMock;
    private readonly Mock<IApplicationValidator> _validatorMock;
    private readonly Mock<IBus> _busMock;
    private readonly ApplicationProcessor _applicationProcessor;
    private readonly Mock<IAdministrationServiceHandlerFactory> _handlerFactoryMock;
    private readonly Mock<IAdministrationServiceHandler> _handlerAdminMock;

    public ApplicationProcessorTests()
    {
        _kycServiceMock = new Mock<IKycService>();
        _validatorMock = new Mock<IApplicationValidator> { CallBase = true };
        _busMock = new Mock<IBus>();
        _handlerFactoryMock = new Mock<IAdministrationServiceHandlerFactory>();
        _handlerAdminMock = new Mock<IAdministrationServiceHandler>();

        SetupHandlers();

        _applicationProcessor = new ApplicationProcessor(
            _kycServiceMock.Object,
            _validatorMock.Object,
            _busMock.Object,
            _handlerFactoryMock.Object
        );
    }

    private void SetupHandlers()
    {
        _handlerFactoryMock.Setup(f => f.GetHandler(ProductCode.ProductOne)).Returns(_handlerAdminMock.Object);
        _handlerFactoryMock.Setup(f => f.GetHandler(ProductCode.ProductTwo)).Returns(_handlerAdminMock.Object);

        _kycServiceMock.Setup(k => k.GetKycReportAsync(It.IsAny<User>()))
                      .ReturnsAsync(Result.Success(new KycReport(Guid.NewGuid(), true)));

        _validatorMock.Setup(v => v.ValidateApplication(It.IsAny<Application>(), out It.Ref<string>.IsAny))
                  .Returns((Application app, out string failureReason) =>
                  {
                      failureReason = "";
                      return true;
                  });
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 20, 5.00)]
    [InlineData(ProductCode.ProductTwo, 20, 5.00)]
    public async Task InvestorCreated_IsPublished_WhenInvestorIsCreatedSuccessfully(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        _handlerAdminMock.Setup(h => h.HandleApplicationAsync(It.IsAny<Application>()))
               .Returns(Task.CompletedTask)
               .Callback<Application>(app =>
               {
                   _busMock.Object.PublishAsync(new InvestorCreated(app.Applicant.Id, new Guid().ToString()));
               });

        // Act
        await _applicationProcessor.Process(application);

        // Assert
        _busMock.Verify(b => b.PublishAsync(It.IsAny<InvestorCreated>()), Times.Once);
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 20, 5.00)]
    [InlineData(ProductCode.ProductTwo, 20, 5.00)]
    public async Task AccountCreated_IsPublished_WhenAccountIsCreatedSuccessfully(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        _handlerAdminMock.Setup(h => h.HandleApplicationAsync(It.IsAny<Application>()))
               .Returns(Task.CompletedTask)
               .Callback<Application>(app =>
               {
                   _busMock.Object.PublishAsync(new AccountCreated(new Guid().ToString(), productCode, new Guid().ToString()));
               });

        // Act
        await _applicationProcessor.Process(application);

        // Assert
        _busMock.Verify(b => b.PublishAsync(It.IsAny<AccountCreated>()), Times.Once);
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 20, 5.00)]
    [InlineData(ProductCode.ProductTwo, 20, 5.00)]
    public async Task ApplicationCompleted_IsPublished_WhenPaymentIsCreatedSuccessfully(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        _handlerAdminMock.Setup(h => h.HandleApplicationAsync(It.IsAny<Application>()))
               .Returns(Task.CompletedTask)
               .Callback<Application>(app =>
               {
                   _busMock.Object.PublishAsync(new ApplicationCompleted(app.Id));
               });

        // Act
        await _applicationProcessor.Process(application);

        // Assert
        _busMock.Verify(b => b.PublishAsync(It.Is<ApplicationCompleted>(e => e.ApplicationId == application.Id)), Times.Once);
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 20, 5.00)]
    [InlineData(ProductCode.ProductTwo, 20, 5.00)]
    public async Task KycCompleted_IsPublished_WhenKycSucceeds(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);
        var reportId = Guid.NewGuid();

        _kycServiceMock.Setup(k => k.GetKycReportAsync(application.Applicant))
                      .ReturnsAsync(Result.Success(new KycReport(reportId, true)));

        // Act
        await _applicationProcessor.Process(application);

        // Assert
        _busMock.Verify(b => b.PublishAsync(It.Is<KycCompleted>(e => e.UserId == application.Applicant.Id && e.ReportId == reportId && e.IsVerified == true)), Times.Once);
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 20, 5.00)]
    [InlineData(ProductCode.ProductTwo, 20, 5.00)]
    public async Task KycFailed_IsPublished_WhenKycFails(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);
        var reportId = Guid.NewGuid();

        _kycServiceMock.Setup(k => k.GetKycReportAsync(application.Applicant))
                      .ReturnsAsync(Result.Success(new KycReport(reportId, false)));

        // Act
        await _applicationProcessor.Process(application);

        // Assert
        _busMock.Verify(b => b.PublishAsync(It.Is<KycFailed>(e => e.UserId == application.Applicant.Id && e.ReportId == reportId)), Times.Once);
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 20, 5.00)]
    [InlineData(ProductCode.ProductTwo, 20, 5.00)]
    public async Task EligibilityCheckCompleted_IsPublished_WhenEligibilityCheckSucceeds(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        // Act
        await _applicationProcessor.Process(application);

        // Assert
        _busMock.Verify(b => b.PublishAsync(It.Is<EligibilityCheckCompleted>(e => e.ApplicationId == application.Id && e.Product == application.ProductCode && e.IsEligible == true)), Times.Once);
    }

    [Theory]
    [InlineData(ProductCode.ProductOne, 15, 0.20)]
    [InlineData(ProductCode.ProductTwo, 15, 0.20)]
    public async Task EligibilityCheckFailed_IsPublished_WhenEligibilityCheckFails(ProductCode productCode, int age, decimal paymentAmount)
    {
        // Arrange
        var application = TestHelper.CreateMockApplication(productCode, DateOnly.FromDateTime(DateTime.Today.AddYears(-age)), paymentAmount);

        _validatorMock.Setup(v => v.ValidateApplication(application, out It.Ref<string>.IsAny))
                      .Returns(false);

        // Act
        await _applicationProcessor.Process(application);

        // Assert
        _busMock.Verify(b => b.PublishAsync(It.Is<EligibilityCheckFailed>(e => e.ApplicationId == application.Id && e.Product == application.ProductCode)), Times.Once);
    }
}