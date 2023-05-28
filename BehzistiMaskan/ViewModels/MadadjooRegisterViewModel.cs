using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class MadadjooRegisterViewModel
    {
        public ClientWaitingApplicantDto ClientWaitingApplicant { get; set; }

        [Display(Name ="استان محل تولد")]
        public int ProvinceOfBirthId { get; set; }

        [Display(Name ="شهرستان محل تولد")]
        public int CountyOfBirthId { get; set; }

        [Display(Name ="بخش محل تولد")]
        public int DistrictOfBirthId { get; set; }

        public IEnumerable<CountyDto> CountyOfBirthList { get; set; }
        public IEnumerable<DistrictDto> DistrictOfBirthList { get; set; }
        public IEnumerable<CityDto> CityOfBirthList { get; set; }



        [Display(Name ="شهرستان")]
        public int HouseCountyId { get; set; }

        [Display(Name ="بخش")]
        public int HouseDistrictId { get; set; }


        [Display(Name ="استان پرونده بهزیستی")]
        public int ProvinceOfBehzistiDocumentId { get; set; }

        [Display(Name ="شهرستان پرونده بهزیستی")]
        public int CountyOfBehzistiDocumentId { get; set; }

        [Display(Name ="بخش پرونده بهزیستی")]
        public int DistrictOfBehzistiDocumentId { get; set; }

        public IEnumerable<CountyDto> CountyOfBehzistiDocumentList { get; set; }
        public IEnumerable<DistrictDto> DistrictOfBehzistiDocumentList { get; set; }
        public IEnumerable<CityDto> CityOfBehzistiDocumentList { get; set; }
        


        public IEnumerable<ProvinceDto> AllProvinces { get; set; }
        public IEnumerable<CountyDto> AllCounties { get; set; }

        public IEnumerable<GenderType> GenderTypes { get; set; }
        public IEnumerable<ClientType> ClientTypes { get; set; }
        public IEnumerable<MarriageType> MarriageTypes { get; set; }
        public IEnumerable<CurrentHouseType> CurrentHouseTypes { get; set; }

        [Required(ErrorMessage = "انتخاب نوع تقاضای مسکن ضروری می باشد")]

        public int RequestTypeBuildingId { get; set; }

        public bool IsRequestWaterExemption { get; set; }
        public bool IsRequestElectricalExemption { get; set; }
        public bool IsRequestGasExemption { get; set; }
        public bool IsRequestProductionLicenseExemption { get; set; }

        public IEnumerable<RequestType> RequestTypes { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        public bool? IsApproved { get; set; }
        public bool? IsRejected { get; set; }

        public IEnumerable<ClientWaitingApplicantLog> ClientWaitingApplicantLogs { get; set; }
    }
}