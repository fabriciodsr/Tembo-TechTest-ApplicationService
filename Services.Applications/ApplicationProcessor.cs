using Services.Applications.Abstractions;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

public class ApplicationProcessor : IApplicationProcessor
{
    private readonly IKycService _kycService;
    private readonly IApplicationValidator _validator;
    private readonly IBus _bus;
    private readonly IAdministrationServiceHandlerFactory _handlerFactory;

    public ApplicationProcessor(IKycService kycService, IApplicationValidator validator, IBus bus, IAdministrationServiceHandlerFactory handlerFactory)
    {
        _kycService = kycService;
        _validator = validator;
        _bus = bus;
        _handlerFactory = handlerFactory;
    }

    public async Task Process(Application application)
    {
        try
        {
            if (!_validator.ValidateApplication(application, out string failureReason))
            {
                await _bus.PublishAsync(new EligibilityCheckFailed(application.Id, application.ProductCode, failureReason));
                return;
            }
            await _bus.PublishAsync(new EligibilityCheckCompleted(application.Id, application.ProductCode, true));

            var kycResult = await _kycService.GetKycReportAsync(application.Applicant);
            if (!kycResult.IsSuccess || !kycResult.Value.IsVerified)
            {
                await _bus.PublishAsync(new KycFailed(application.Applicant.Id, kycResult.Value.Id));
                return;
            }
            await _bus.PublishAsync(new KycCompleted(application.Applicant.Id, kycResult.Value.Id, kycResult.Value.IsVerified));
            application.Applicant.IsVerified = true;

            var handler = _handlerFactory.GetHandler(application.ProductCode);
            await handler.HandleApplicationAsync(application);
        }
        catch (Exception ex)
        {
            await _bus.PublishAsync(new ApplicationFailed(application.Id, application.ProductCode, ex.Message));
        }
    }
}