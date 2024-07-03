using Services.Applications.Abstractions;
using Services.Applications.Model;
using Services.Common.Abstractions.Helpers;
using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Applications
{
    public class ApplicationValidator : IApplicationValidator
    {
        private readonly EligibilitySettings _eligibilitySettings;

        public ApplicationValidator(EligibilitySettings eligibilitySettings)
        {
            _eligibilitySettings = eligibilitySettings;
        }

        public bool ValidateApplication(Application application, out string failureReason)
        {
            failureReason = string.Empty;
            var userAge = AgeHelper.CalculateAge(application.Applicant.DateOfBirth);
            var productEligibility = application.ProductCode == ProductCode.ProductOne
                ? _eligibilitySettings.ProductOne
                : _eligibilitySettings.ProductTwo;

            if (userAge < productEligibility.MinAge || userAge > productEligibility.MaxAge)
            {
                failureReason = "Applicant age does not meet the product's eligibility criteria.";
                return false;
            }

            if (application.Payment.Amount.Amount < productEligibility.MinPayment)
            {
                failureReason = "Payment does not meet the minimum requirement.";
                return false;
            }

            return true;
        }
    }
}
