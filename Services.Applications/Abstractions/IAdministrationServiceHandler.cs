using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Applications.Abstractions
{
    public interface IAdministrationServiceHandler
    {
        Task HandleApplicationAsync(Application application);
    }
}
