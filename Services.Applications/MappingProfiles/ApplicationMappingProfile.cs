using AutoMapper;
using Services.AdministratorOne.Abstractions.Model;
using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Applications.MappingProfiles
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Application, CreateInvestorRequest>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Applicant.Forename))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Applicant.Surname))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Applicant.DateOfBirth.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.Nino, opt => opt.MapFrom(src => src.Applicant.Nino))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.ProductCode.ToString()))
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(src => src.Applicant.Id.ToString()))
                .ForMember(dest => dest.InitialPayment, opt => opt.MapFrom(src => src.Payment.Amount.Amount))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Applicant.BankAccounts.FirstOrDefault().AccountNumber))
                .ForMember(dest => dest.SortCode, opt => opt.MapFrom(src => src.Applicant.BankAccounts.FirstOrDefault().SortCode))
                .ForMember(dest => dest.Addressline1, opt => opt.MapFrom(src => src.Applicant.Addresses.FirstOrDefault().Addressline1))
                .ForMember(dest => dest.Addressline2, opt => opt.MapFrom(src => src.Applicant.Addresses.FirstOrDefault().Addressline2))
                .ForMember(dest => dest.Addressline3, opt => opt.MapFrom(src => src.Applicant.Addresses.FirstOrDefault().Addressline3))
                .ForMember(dest => dest.Addressline4, opt => opt.MapFrom(src => src.Applicant.Addresses.FirstOrDefault().Country))
                .ForMember(dest => dest.PostCode, opt => opt.MapFrom(src => src.Applicant.Addresses.FirstOrDefault().PostCode))
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.MobileNumber, opt => opt.Ignore());
        }
    }
}
