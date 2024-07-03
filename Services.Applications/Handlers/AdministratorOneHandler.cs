using AutoMapper;
using Services.AdministratorOne.Abstractions;
using Services.AdministratorOne.Abstractions.Model;
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
    public class AdministratorOneHandler : IAdministrationServiceHandler
    {
        private readonly IAdministrationService _service;
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public AdministratorOneHandler(IAdministrationService service, IBus bus, IMapper mapper)
        {
            _service = service;
            _bus = bus;
            _mapper = mapper;
        }

        public async Task HandleApplicationAsync(Application application)
        {
            try
            {
                var request = _mapper.Map<CreateInvestorRequest>(application);
                var response = _service.CreateInvestor(request);

                await _bus.PublishAsync(new InvestorCreated(application.Applicant.Id, response.InvestorId));
                await _bus.PublishAsync(new AccountCreated(response.InvestorId, application.ProductCode, response.AccountId));
                await _bus.PublishAsync(new ApplicationCompleted(application.Id));
            }
            catch (AdministratorException ex)
            {
                switch (ex.Code)
                {
                    case ErrorCodes.InvestorError:
                        await _bus.PublishAsync(new InvestorCreationFailed(application.Id, application.ProductCode, "Failed to create investor"));
                        break;
                    case ErrorCodes.AccountError:
                        await _bus.PublishAsync(new AccountCreationFailed(application.Id, string.Empty, application.ProductCode, "Failed to create account"));
                        break;
                    case ErrorCodes.PaymentError:
                        await _bus.PublishAsync(new ApplicationFailed(application.Id, application.ProductCode, "Failed to process payment"));
                        break;
                }
            }
        }
    }
}
