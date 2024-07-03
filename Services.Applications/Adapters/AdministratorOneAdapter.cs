using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.AdministratorOne.Abstractions;
using Services.AdministratorOne.Abstractions.Model;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications
{
    public class AdministratorOneAdapter : IAdministrationService
    {
        public AdministratorOneAdapter()
        {
        }

        public CreateInvestorResponse CreateInvestor(CreateInvestorRequest request)
        {
            // TODO: Handle the implementation for a third party service or something related

            return new CreateInvestorResponse
            {
                Reference = request.Reference,
                AccountId = Guid.NewGuid().ToString(),
                InvestorId = Guid.NewGuid().ToString(),
                PaymentId = Guid.NewGuid().ToString()
            };
        }
    }
}
