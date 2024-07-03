using Services.AdministratorTwo.Abstractions;
using Services.Applications.Abstractions;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Applications.Handlers
{
    public class AdministratorTwoHandler : IAdministrationServiceHandler
    {
        private readonly IAdministrationService _service;
        private readonly IBus _bus;

        public AdministratorTwoHandler(IAdministrationService service, IBus bus)
        {
            _service = service;
            _bus = bus;
        }

        public async Task HandleApplicationAsync(Application application)
        {
            var investorId = await _service.CreateInvestorAsync(application.Applicant);
            if (!investorId.IsSuccess)
            {
                await _bus.PublishAsync(new InvestorCreationFailed(application.Id, application.ProductCode, "Failed to create investor"));
                return;
            }
            await _bus.PublishAsync(new InvestorCreated(application.Applicant.Id, investorId.Value.ToString()));

            var accountId = await _service.CreateAccountAsync(investorId.Value, application.ProductCode);
            if (!accountId.IsSuccess)
            {
                await _bus.PublishAsync(new AccountCreationFailed(application.Id, investorId.Value.ToString(), application.ProductCode, "Failed to create account"));
                return;
            }
            await _bus.PublishAsync(new AccountCreated(investorId.Value.ToString(), application.ProductCode, accountId.Value.ToString()));

            var paymentResult = await _service.ProcessPaymentAsync(accountId.Value, application.Payment);
            if (!paymentResult.IsSuccess)
            {
                await _bus.PublishAsync(new ApplicationFailed(application.Id, application.ProductCode, "Failed to process payment"));
                return;
            }
            await _bus.PublishAsync(new ApplicationCompleted(application.Id));
        }
    }
}
