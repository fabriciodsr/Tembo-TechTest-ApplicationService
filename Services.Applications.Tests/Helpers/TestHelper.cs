using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Applications.Tests.Helpers
{
    public static class TestHelper
    {
        public static Application CreateMockApplication(ProductCode productCode, DateOnly dateOfBirth, decimal paymentAmount)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Forename = "John",
                Surname = "Doe",
                DateOfBirth = dateOfBirth,
                IsVerified = false,
                Nino = "AB123456C",
                Addresses = new[]
                {
                    new Address
                    {
                        Addressline1 = "123 Main St",
                        Addressline2 = "Apt 4",
                        Addressline3 = "Suburb, City",
                        PostCode = "12345",
                        Country = "Test"
                    }
                },
                BankAccounts = new[]
                {
                    new BankAccount
                    {
                        SortCode = "123456",
                        AccountNumber = "654321"
                    }
                }
            };

            return new Application
            {
                Id = Guid.NewGuid(),
                Applicant = user,
                ProductCode = productCode,
                Payment = new Payment(user.BankAccounts.FirstOrDefault(), new Money("GBP", paymentAmount))
            };
        }
    }

}
