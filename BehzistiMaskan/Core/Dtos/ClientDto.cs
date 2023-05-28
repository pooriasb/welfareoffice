using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models;
using MD.PersianDateTime;

namespace BehzistiMaskan.Core.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }

        [Display(Name = "سرپرست خانوار")]
        public bool IsHouseHold { get; set; }

        [Required(ErrorMessage = "انتخاب نوع پرونده مددجو ضروری می باشد!")]
        [Display(Name = "نوع مددجو")]
        public int? ClientTypeId { get; set; }

        //public ClientType ClientType { get; set; }
        public string ClientTypeStr { get; set; }

        [Display(Name = "توضیحات نوع مددجو")]
        public string ClientTypeDescription { get; set; }

        [Required(ErrorMessage = "انتخاب معاونت مددجو ضروری می باشد!")]
        [Display(Name = "نوع معاونت")]
        public int? AssistanceTypeId { get; set; }

        public string AssistanceTypeStr { get; set; }

        [Display(Name = "درآمد ماهانه")]
        public long? MonthlyIncome { get; set; }

        public bool? IsIncludedInComprehensiveReport { get; set; }

        public bool? IsDeleted { get; set; }

        [Required(ErrorMessage = "انتخاب شهر یا روستا ضروری می باشد!")]
        [Display(Name = "شهر یا روستا (پرونده بهزیستی)")]
        public int CityId { get; set; }

        public string CityName { get; set; }

        [Display(Name = "بخش")]
        public int? DistrictId { get; set; }

        public string DistrictName { get; set; }

        [Display(Name = "شهرستان")]
        public int? CountyId { get; set; }

        public string CountyName { get; set; }

        [Display(Name = "استان")]
        public int? ProvinceId { get; set; }

        public string ProvinceName { get; set; }

        [Display(Name = "کد مددجو در سامانه کشوری")]
        public string GlobalBehzistiUiCode { get; set; }

        [Display(Name = "شماره پرونده مددجو")]
        public string BehzistiCode { get; set; }

        [Display(Name = "تعداد معلولین در خانواده")]
        public int? NumberOfDisabledInFamily { get; set; }

        [Display(Name = "تاریخ ثبت اولیه")]
        [DataType(DataType.Date)]
        public PersianDateTime? CreatedDate { get; set; }

        [Display(Name = "تاریخ آخرین ویرایش")]
        [DataType(DataType.Date)]
        public PersianDateTime? UpdatedDate { get; set; }

        public PersonDto Person { get; set; }

        public bool HasClientUser { get; set; }

        public ClientState ClientState { get; set; }
        public string ClientStateStr { get; set; }

        public FormEmtiazBandi FormEmtiazBandi { get; set; }

        public string GenderTypeStr { get; set; }

        public string MarriageTypeStr { get; set; }

        public string BodyType { get; set; }

        public List<string> Requests { get; set; }

        public bool OnlyRequestExemption { get; set; }

        public bool IsRequestAnyExemption { get; set; }

        public string CurrentHousingTypeStr{ get; set; }

        public string CurrentHouseAddress{ get; set; }

        public string BuildingHouseAddress{ get; set; }

        public string SelectedFormsStr { get; set; }

    }
}