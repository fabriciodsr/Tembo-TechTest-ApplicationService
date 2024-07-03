using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Applications.Model
{
    public class EligibilitySettings
    {
        public ProductEligibility ProductOne { get; set; }
        public ProductEligibility ProductTwo { get; set; }
    }
}
