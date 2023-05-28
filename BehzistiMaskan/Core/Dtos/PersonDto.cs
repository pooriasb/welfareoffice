using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;
using MD.PersianDateTime;

namespace BehzistiMaskan.Core.Dtos
{
    public class PersonDto
    {
        public int Id { get; set; }

        [Display(Name = "تاریخ ثبت اولیه")]
        public PersianDateTime? CreatedDate { get; set; }

        [Display(Name = "تاریخ آخرین ویرایش")]
        public PersianDateTime? UpdatedDate { get; set; }

        [Required(ErrorMessage = "نام ضروری می باشد!")]
        [Display(Name = "نام")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "نام خانوادگی ضروری می باشد!")]
        [Display(Name = "نام خانوادگی")]
        [StringLength(255)]
        public string Family { get; set; }

        [Required(ErrorMessage = "نام پدر ضروری می باشد!")]
        [Display(Name = "نام پدر")]
        [StringLength(255)]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "نام مادر ضروری می باشد!")]
        [Display(Name = "نام مادر")]
        [StringLength(255)]
        public string MotherName { get; set; }

        [Required(ErrorMessage = "کد ملی ضروری می باشد!")]
        [Display(Name = "کد ملی")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی وارد شده صحیح نمی باشد")]
        public string NationalCode { get; set; }

        [Required(ErrorMessage = "شماره شناسنامه ضروری می باشد!")]
        [Display(Name = "شماره شناسنامه")]
        [StringLength(40)]
        public string BirthCertificateNo { get; set; }

        [Display(Name = "شماره مسلسل شناسنامه")]
        [StringLength(40)]
        public string BirthCertificateMosalsal { get; set; }

        [Display(Name = "توضیحات شناسنامه")]
        [StringLength(3000)]
        public string BirthCertificateDescription { get; set; }

        [Required(ErrorMessage = "تاریخ تولد ضروری می باشد!")]
        [Display(Name = "تاریخ تولد")]
        [DataType(DataType.Date, ErrorMessage = "تاریخ به درستی وارد نشده است")]
        public string Birthdate { get; set; }

        [Required(ErrorMessage = "جنسیت ضروری می باشد!")]
        [Display(Name = "جنسیت")]
        public int? GenderTypeId { get; set; }

        [Required(ErrorMessage = "وضعیت تاهل ضروری می باشد!")]
        [Display(Name = "وضعیت تاهل")]
        public int? MarriageTypeId { get; set; }

        [Display(Name = "تعداد فرزندان")]
        public int? NumberOfChildren { get; set; }

        [Required(ErrorMessage = "شهر محل تولد ضروری می باشد!")]
        [Display(Name = "شهر محل تولد")]
        public int? CityOfBirthId { get; set; }

        public string CityOfBirthName { get; set; }

        [Display(Name = "بخش محل تولد")]
        public int? DistrictOfBirthId { get; set; }

        public string DistrictOfBirthName { get; set; }

        [Display(Name = "شهرستان محل تولد")]
        public int? CountyOfBirthId { get; set; }

        public string CountyOfBirthName { get; set; }

        [Display(Name = "استان محل تولد")]
        public int? ProvinceOfBirthId { get; set; }

        public string ProvinceOfBirthName { get; set; }

        [Display(Name = "آیا معلول است؟")]
        public bool IsDisabled { get; set; }

        public bool? IsDeleted { get; set; }

        public bool IsClient { get; set; }
    }
}