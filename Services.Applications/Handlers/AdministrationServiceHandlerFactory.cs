using Microsoft.Extensions.DependencyInjection;
using Services.Applications.Abstractions;
using Services.Applications.Handlers;
using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Applications.Handlers
{
    public class AdministrationServiceHandlerFactory : IAdministrationServiceHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AdministrationServiceHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAdministrationServiceHandler GetHandler(ProductCode productCode)
        {
            switch (productCode)
            {
                case ProductCode.ProductOne:
                    return _serviceProvider.GetRequiredService<AdministratorOneHandler>();
                case ProductCode.ProductTwo:
                    return _serviceProvider.GetRequiredService<AdministratorTwoHandler>();
                default:
                    throw new KeyNotFoundException("No handler registered for given product code.");
            }
        }
    }
}
