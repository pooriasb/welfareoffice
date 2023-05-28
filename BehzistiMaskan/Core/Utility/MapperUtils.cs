using System.Linq;
using AutoMapper;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.ReportBuilder;
using BehzistiMaskan.ViewModels;
using MD.PersianDateTime;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BehzistiMaskan.Core.Utility
{
    public static class MapperUtils
    {
        public static readonly MapperConfiguration MapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Benefactor, BenefactorSimpleDto>()
                .ForMember(b => b.ProvinceName, opt => opt.MapFrom(src => src.Province.Name))
                .ForMember(b => b.CountyName, opt => opt.MapFrom(src => src.County != null ? src.County.Name : null))
                .ForMember(b=>b.WillHelpWithCash,opt=>opt.MapFrom(src=>src.WillHelpWithCash?"دارد":"ندارد"))
                .ForMember(b=>b.WillHelpWithGift,opt=>opt.MapFrom(src=>src.WillHelpWithGift?"دارد":"ندارد"))
                .ForMember(b=>b.WillHelpWithService,opt=>opt.MapFrom(src=>src.WillHelpWithService?"دارد":"ندارد"))
                .ForMember(b=>b.HelpToCreateAHouse, opt=>opt.MapFrom(src=>src.HelpToCreateAHouse?"دارد":"ندارد"))
                .ForMember(b=>b.HelpToBuyAHouse, opt=>opt.MapFrom(src=>src.HelpToBuyAHouse?"دارد":"ندارد"))
                .ForMember(b=>b.HelpToFixAHouse, opt=>opt.MapFrom(src=>src.HelpToFixAHouse?"دارد":"ندارد"))
                .ForMember(b=>b.HelpToPayMonthlyRental, opt=>opt.MapFrom(src=>src.HelpToPayMonthlyRental?"دارد":"ندارد"))
                .ForMember(b=>b.HelpToPayMortgageMoney, opt=>opt.MapFrom(src=>src.HelpToPayMortgageMoney?"دارد":"ندارد"))
                .ForMember(b=>b.HelpToPayLoanQuarter, opt=>opt.MapFrom(src=>src.HelpToPayLoanQuarter?"دارد":"ندارد"))
                .ForMember(b=>b.IsContinuum, opt=>opt.MapFrom(src=>src.IsContinuum?"بله":"خیر"))
                .ForMember(b=>b.WantOnlinePayment, opt=>opt.MapFrom(src=>src.WantOnlinePayment?"بله":"خیر"))
                
                ;

            cfg.CreateMap<BenefactorDto, Benefactor>()
                .ForMember(b => b.Birthdate, opt => opt.ConvertUsing(new PersianToGregorianDateConverter()))
                ;

            cfg.CreateMap<Benefactor, BenefactorDto>()
                .ForMember(b => b.Birthdate, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                ;

            cfg.CreateMap<Province, ProvinceDto>();
            cfg.CreateMap<ProvinceDto, Province>()
                .ForMember(m => m.Id, opt => opt.Ignore());

            cfg.CreateMap<County, CountyDto>()
                .ForMember(m => m.ProvinceName, member => member.MapFrom(src => src.Province.Name));
            cfg.CreateMap<CountyDto, County>()
                .ForMember(m => m.Id, opt => opt.Ignore());

            cfg.CreateMap<District, DistrictDto>()
                .ForMember(d => d.CountyName, opt => opt.MapFrom(src => src.County.Name));
            cfg.CreateMap<DistrictDto, District>()
                .ForMember(d => d.Id, opt => opt.Ignore());

            cfg.CreateMap<City, CityDto>()
                .ForMember(m => m.DistrictName, opt => opt.MapFrom(src => src.District.Name));
            cfg.CreateMap<CityDto, City>()
                .ForMember(c => c.Id, opt => opt.Ignore());

            cfg.CreateMap<Client, ClientDto>()
                .ForMember(c => c.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(c => c.DistrictName, opt => opt.MapFrom(src => src.City.District.Name))
                .ForMember(c => c.CountyName, opt => opt.MapFrom(src => src.City.District.County.Name))
                .ForMember(c => c.CityId, opt => opt.MapFrom(src => src.City.Id))
                .ForMember(c => c.DistrictId, opt => opt.MapFrom(src => src.City.DistrictId))
                .ForMember(c => c.CountyId, opt => opt.MapFrom(src => src.City.District.CountyId))
                .ForMember(c => c.ProvinceId, opt => opt.MapFrom(src => src.City.District.County.ProvinceId))
                .ForMember(p => p.CreatedDate, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(p => p.UpdatedDate, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(c => c.Person, opt => opt.MapFrom(src => src.Person))
                .ForMember(c => c.IsHouseHold, opt => opt.MapFrom(c => c.IsHouseHold ?? false))
                .ForMember(c => c.BodyType, opt => opt.MapFrom(c => c.Person.IsDisabled == true ? "معلول" : "سالم"))
                .ForMember(c => c.Requests, opt => opt.MapFrom(src => src.ClientRequests.Select(request => request.RequestType.PersianShortTitle).ToList()))
                .ForMember(c => c.OnlyRequestExemption, opt => opt.MapFrom(src => src.ClientRequests.Any(request => request.RequestTypeId == 0)))
                .ForMember(c => c.IsRequestAnyExemption, opt => opt.MapFrom(src => src.ClientRequests.Any(request => request.RequestType.IsExemption)))
                .ForMember(c => c.ClientStateStr, opt => opt.MapFrom(c => c.ClientState.ClientStateTypeE.ToPersianString()))
                .ForMember(c => c.GenderTypeStr, opt => opt.MapFrom(c => c.Person.GenderType != null ? c.Person.GenderType.Name : ""))
                .ForMember(c => c.ClientTypeStr, opt => opt.MapFrom(c => c.ClientType != null ? c.ClientType.Name : ""))
                .ForMember(c => c.AssistanceTypeStr, opt => opt.MapFrom(c => c.AssistanceType != null ? c.AssistanceType.Name : ""))
                .ForMember(c => c.MarriageTypeStr, opt => opt.MapFrom(c => c.Person.MarriageType != null ? c.Person.MarriageType.Name : ""))
                .ForMember(c => c.CurrentHousingTypeStr, opt => opt.MapFrom(c => c.CurrentHousing != null ? c.CurrentHousing.CurrentHouseType.Name : ""))
                .ForMember(c => c.CurrentHouseAddress, opt => opt.MapFrom(c => c.CurrentHousing != null ? c.CurrentHousing.AddressCurrentHouse : ""))
                .ForMember(c => c.BuildingHouseAddress, opt => opt.MapFrom(c => c.CurrentHousing != null ? c.CurrentHousing.Address : ""))
                .ForMember(c => c.SelectedFormsStr, opt => opt.MapFrom(c => c.ClientForms != null && c.ClientForms.Any() ?
                    c.ClientForms.Aggregate("", (current, clientForm) => current + " ، " + clientForm.Form.Name) : ""))

                ;




            //cfg.CreateMap<Client, ExemptionReportSimpleDto>()
            //    .ForMember(er => er.ClientId, opt => opt.MapFrom(src => src.Id))
            //    .ForMember(er => er.Name, opt => opt.MapFrom(src => src.Person.Name))
            //    .ForMember(er => er.Family, opt => opt.MapFrom(src => src.Person.Family))
            //    .ForMember(er => er.NationalCode, opt => opt.MapFrom(src => src.Person.NationalCode))
            //    .ForMember(er => er.CountyName, opt => opt.MapFrom(src => src.City.District.County.Name))
            //    .ForMember(er => er.Address, opt => opt.MapFrom(src => src.CurrentHousing.Address))
            //    .ForMember(er => er.HasGetLetterWater, opt => opt.MapFrom(src => src.ClientRequests.Any(cr=>cr.RequestTypeId == (int)ModelEnums.ClientRequestTypeE.ExemptionWater && cr.GetLetters.Any())))
            //    .ForMember(er => er.GetLetterDateWater, opt => opt.MapFrom(src => src.ClientRequests.Any(cr=>cr.RequestTypeId == (int)ModelEnums.ClientRequestTypeE.ExemptionWater && cr.GetLetters.Any())?
            //        src.ClientRequests.Single(cr=>cr.RequestTypeId == (int)ModelEnums.ClientRequestTypeE.ExemptionWater && cr.GetLetters.Any()).GetLetters.Aggregate(gl=>gl.) :
            //        ))

            //    //.ForMember(er => er.BenefitAmountWater, opt => opt.MapFrom(src => src.ClientRequests.Where(cr => cr.RequestType.Name == ClientRequestTypeStr.ExemptionWater).Sum(cr => cr.ExemptionBenefits.Sum(eb => eb.BenefitAmount))))
            //    //.ForMember(er => er.GetLetterDateWater, opt => opt.MapFrom(src =>
            //    //    src.ClientRequests.SingleOrDefault(cr => cr.RequestType.Name == ClientRequestTypeStr.ExemptionWater)
            //    //        .GetLetters.Select(gl => gl.LetterDate).ToString()))


            //    ;

            cfg.CreateMap<ClientWaitingApplicant, ClientDto>()
                .ForMember(c => c.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(c => c.DistrictName, opt => opt.MapFrom(src => src.City.District.Name))
                .ForMember(c => c.CountyName, opt => opt.MapFrom(src => src.City.District.County.Name))
                .ForMember(c => c.CityId, opt => opt.MapFrom(src => src.City.Id))
                .ForMember(c => c.DistrictId, opt => opt.MapFrom(src => src.City.DistrictId))
                .ForMember(c => c.CountyId, opt => opt.MapFrom(src => src.City.District.CountyId))
                .ForMember(c => c.ProvinceId, opt => opt.MapFrom(src => src.City.District.County.ProvinceId))
                .ForMember(p => p.CreatedDate, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(p => p.UpdatedDate, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                ;


            cfg.CreateMap<FormEmtiazBandi, FormEmtiazBandiDto>()
                .ForMember(f => f.DateOfKarshenasShahrestanApprove, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(f => f.DateOfModirShahrestanApprove, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(f => f.DateOfMoavenMosharekatAssistanceApprove, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(f => f.DateOfRelatedAssistanceApprove, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(f => f.DateOfModirKolApprove, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                ;

            cfg.CreateMap<ClientWaitingApplicant, ClientWaitingApplicantSimpleDto>()
                .ForMember(c => c.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(c => c.DistrictName, opt => opt.MapFrom(src => src.City.District.Name))
                .ForMember(c => c.CountyName, opt => opt.MapFrom(src => src.City.District.County.Name))
                .ForMember(c => c.Requests, opt => opt.MapFrom(src => src.Requests.Select(request => request.RequestType.PersianShortTitle).ToList()))
                ;

            cfg.CreateMap<ClientWaitingApplicant, ClientWaitingApplicantDto>()
                .ForMember(c => c.Birthdate, opt => opt.MapFrom(src => new PersianDateTime(src.Birthdate)))
                ;

            cfg.CreateMap<ClientWaitingApplicantDto, ClientWaitingApplicant>()
            .ForMember(c => c.Id, opt => opt.Ignore())
            .ForMember(c => c.CreatedDate, opt => opt.Ignore())
            ;

            cfg.CreateMap<ClientDto, Client>()
                .ForMember(c => c.Id, opt => opt.Ignore())
                .ForMember(c => c.CreatedDate, opt => opt.ConvertUsing(new PersianToGregorianDateConverter()))
                .ForMember(c => c.UpdatedDate, opt => opt.ConvertUsing(new PersianToGregorianDateConverter()))
                .ForMember(c => c.Person, opt => opt.Ignore())
                ;

            //cfg.RecognizePrefixes("Person");
            //cfg.CreateMap<ClientDto, Person>()
            //    .ForMember(p => p.Id, opt => opt.Ignore())
            //    .ForMember(c => c.Birthdate, opt => opt.ConvertUsing(new PersianToGregorianDateConverter()))
            //    .ForMember(c => c.CreatedDate, opt => opt.ConvertUsing(new PersianToGregorianDateConverter()))
            //    .ForMember(c => c.UpdatedDate, opt => opt.ConvertUsing(new PersianToGregorianDateConverter()))
            //    ;

            cfg.CreateMap<Person, PersonDto>()
                .ForMember(p => p.IsClient, opt => opt.MapFrom(person => person.Client != null))
                .ForMember(p => p.ProvinceOfBirthId, opt => opt.MapFrom(person => person.CityOfBirthId != null ? person.CityOfBirth.District.County.ProvinceId : 0))
                .ForMember(p => p.ProvinceOfBirthName, opt => opt.MapFrom(person => person.CityOfBirthId != null ? person.CityOfBirth.District.County.Province.Name : null))
                .ForMember(p => p.CountyOfBirthId, opt => opt.MapFrom(person => person.CityOfBirthId != null ? person.CityOfBirth.District.CountyId : 0))
                .ForMember(p => p.CountyOfBirthName, opt => opt.MapFrom(person => person.CityOfBirthId != null ? person.CityOfBirth.District.County.Name : null))
                .ForMember(p => p.DistrictOfBirthId, opt => opt.MapFrom(person => person.CityOfBirthId != null ? person.CityOfBirth.District.Id : 0))
                .ForMember(p => p.DistrictOfBirthName, opt => opt.MapFrom(person => person.CityOfBirthId != null ? person.CityOfBirth.District.Name : null))
                .ForMember(p => p.CityOfBirthName, opt => opt.MapFrom(person => person.CityOfBirthId != null ? person.CityOfBirth.Name : null))
                .ForMember(p => p.Birthdate, opt => opt.ConvertUsing(new GregorianToPersianStringDateConverter()))
                .ForMember(p => p.CreatedDate, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(p => p.UpdatedDate, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(p => p.IsDisabled, opt => opt.MapFrom(p => p.IsDisabled ?? false))
                ;

            cfg.CreateMap<PersonDto, Person>()
                //.ForMember(c=>c.Id, opt=>opt.Ignore())
                .ForMember(c => c.CreatedDate, opt => opt.ConvertUsing(new PersianToGregorianDateConverter()))
                .ForMember(c => c.UpdatedDate, opt => opt.ConvertUsing(new PersianToGregorianDateConverter()))
                .ForMember(c => c.Birthdate, opt => opt.ConvertUsing(new PersianStringDateToGregorianDateConverter()))

                ;

            cfg.CreateMap<CurrentHousing, CurrentHousing>();

            cfg.CreateMap<ContactInfo, ContactInfo>();

            cfg.CreateMap<BankInfo, BankInfo>();

            cfg.CreateMap<Form, FormDto>()
                .ForMember(f => f.AccessLevelStr,
                    opt => opt.MapFrom(c =>
                        c.FormAccessLevels.Aggregate("", (current, accessLevel) => current + (accessLevel.County.Name + "، "))))
                .ForMember(f => f.CoOrganizationRoleStr,
                    opt => opt.MapFrom(c =>
                         c.FormCoOrganizationRoles.Aggregate("", (current, coOrg) => current + (coOrg.CoOrganizationType.Name + "، "))))
                .ForMember(f => f.CreatedAt, opt => opt.ConvertUsing(new GregorianToPersianStringDateConverter()))
                .ForMember(f => f.FormStatus, opt => opt.MapFrom(x => x.IsEnabled ? "فعال" : "غیر فعال"))
                ;

            cfg.CreateMap<FormDto, Form>()
                .ForMember(f => f.CreatedAt, opt => opt.Ignore())
                ;

            cfg.CreateMap<JsonFormField, Field>()
                .ForMember(f => f.IsRequired, opt => opt.MapFrom(x => x.IsRequired ?? false))
                .ForMember(f => f.Title, opt => opt.MapFrom(jf => jf.Title.IsNullOrWhiteSpace() ? jf.FieldTemplateName : jf.Title))
                ;
            cfg.CreateMap<Field, JsonFormField>();

            cfg.CreateMap<FamilyRelation, FamilyRelation>();

            cfg.CreateMap<IdentityRole, UserRoleDto>()
                .ForMember(m => m.PersianName, opt => opt.MapFrom(r => r.Name.ConvertRoleNameToPersian()));

            cfg.CreateMap<UserInfo, UserInfo>()
                .ForMember(u => u.Id, opt => opt.Ignore());


            cfg.CreateMap<Form, DashboardFormSimpleData>()
                .ForMember(m => m.Progress, opt => opt.UseDestinationValue())
                .ForMember(m => m.Status, opt => opt.MapFrom(f => f.FormStatus));


            cfg.CreateMap<Report, ReportSimpleDto>()
                .ForMember(c => c.CreatedAt, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()))
                .ForMember(c => c.UpdateAt, opt => opt.ConvertUsing(new GregorianToPersianDateConverter()));

            cfg.CreateMap<Report, Report>();

            cfg.CreateMap<ClientStateLog, ClientStateLogDto>()
                .ForMember(cs => cs.LogDate, opt => opt.MapFrom(csl => new PersianDateTime(csl.LogDate)))
                .ForMember(cs => cs.ClientStateTypeStr, opt => opt.MapFrom((csl => csl.ClientStateTypeE.ToPersianString())))
                ;



        });


    }

    public class ClientStateToPersianStr : IValueConverter<ClientState, string>
    {
        public string Convert(ClientState sourceMember, ResolutionContext context)
        {
            return sourceMember == null ? "" : sourceMember.ClientStateTypeE.ToPersianString();
        }
    }
}