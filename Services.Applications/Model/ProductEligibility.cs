using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Applications.Model
{
    public class ProductEligibility
    {
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public decimal MinPayment { get; set; }
    }
}
