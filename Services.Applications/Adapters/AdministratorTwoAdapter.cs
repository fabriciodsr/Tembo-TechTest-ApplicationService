using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.AdministratorTwo.Abstractions;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications
{
    public class AdministratorTwoAdapter : IAdministrationService
    {
        public AdministratorTwoAdapter()
        {
        }

        public Task<Result<Guid>> CreateInvestorAsync(User user)
        {
            // TODO: Handle the implementation for a third party service or something related
            return Task.FromResult(Result.Success(Guid.NewGuid()));
        }

        public Task<Result<Guid>> CreateAccountAsync(Guid investorId, ProductCode productCode)
        {
            // TODO: Handle the implementation for a third party service or something related
            return Task.FromResult(Result.Success(Guid.NewGuid()));
        }

        public Task<Result<Guid>> ProcessPaymentAsync(Guid accountId, Payment payment)
        {
            // TODO: Handle the implementation for a third party service or something related
            return Task.FromResult(Result.Success(Guid.NewGuid()));
        }
    }
}
