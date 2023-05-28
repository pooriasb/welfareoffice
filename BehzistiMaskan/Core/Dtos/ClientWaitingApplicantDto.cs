using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;
using MD.PersianDateTime;

namespace BehzistiMaskan.Core.Dtos
{
    public class ClientWaitingApplicantDto
    {
        
        public int Id { get; set; }


        [Required(ErrorMessage = "وارد کردن نام ضروری می باشد")]
        [MinLength(3, ErrorMessage = "نام به صورت صحیح وارد نشده است")]
        [Display(Name = "نام")]
        public string Name { get; set; }

        [Required(ErrorMessage = "وارد کردن نام خانوادگی ضروری می باشد")]
        [MinLength(2, ErrorMessage = "نام خانوادگی به صورت صحیح وارد نشده است")]
        [Display(Name = "نام خانوادگی")]
        public string Family { get; set; }

        [Required(ErrorMessage = "وارد کردن نام پدر ضروری می باشد")]
        [MinLength(3, ErrorMessage = "نام پدر به صورت صحیح وارد نشده است")]
        [Display(Name = "نام پدر")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "وارد کردن نام مادر ضروری می باشد")]
        [MinLength(3, ErrorMessage = "نام مادر به صورت صحیح وارد نشده است")]
        [Display(Name = "نام مادر")]
        public string MotherName { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "وارد کردن کد ملی ضروری می باشد")]
        [StringLength(10, ErrorMessage = "کد ملی به صورت صحیح وارد نشده است", MinimumLength = 10)]
        [RegularExpression("[0-9]+", ErrorMessage = "کد ملی فقط شامل اعداد بین 0 تا 9 می باشد")]
        public string NationalCode { get; set; }


        [Display(Name = "شماره شناسنامه")]
        [Required(ErrorMessage = "وارد کردن شماره شناسنامه ضروری می باشد")]
        [RegularExpression("[0-9]+", ErrorMessage = "شماره شناسنامه فقط شامل اعداد بین 0 تا 9 می باشد")]
        public string BirthCertificateNo { get; set; }

        [Display(Name = "شماره مسلسل شناسنامه")]
        public string BirthCertificateMosalsal { get; set; }

        [Display(Name = "توضیحات شناسنامه")]
        public string BirthCertificateDescription { get; set; }

        [Display(Name = "تاریخ تولد")]
        [DataType(DataType.Date, ErrorMessage = "تاریخ به صورت صحیح وارد نشده است")]
        [Required(ErrorMessage = "لطفا تاریخ تولد را انتخاب کنید")]
        public string Birthdate { get; set; }

        [Required(ErrorMessage = "وارد کردن جنسیت ضروری می باشد")]
        [Display(Name = "جنسیت")]
        public int GenderTypeId { get; set; }

        public GenderType GenderType { get; set; }

        [Required(ErrorMessage = "وارد کردن وضعیت تاهل ضروری می باشد")]
        [Display(Name = "وضعیت تاهل")]
        public int MarriageTypeId { get; set; }

        public MarriageType MarriageType { get; set; }

        [Display(Name = "تعداد فرزندان")]
        public int? NumberOfChildren { get; set; }

        [Display(Name = "شهر محل تولد")]
        public int CityOfBirthId { get; set; }
        public City CityOfBirth { get; set; }

        [Display(Name = "آیا معلول می باشد؟")]
        public bool IsDisabled { get; set; }
        //پایان مشخصات فردی



        // مشخصات پرونده بهزیستی
        [Display(Name = "درآمد ماهانه")]
        public long? MonthlyIncome { get; set; }

        [Display(Name = "شهر یا روستایی که در آن پرونده دارید")]
        public int CityId { get; set; }
        public City City { get; set; }

        // تعداد معلولین در خانواده
        [Display(Name = "تعداد معلولان در خانواده")]
        public int NumberOfDisabledInFamily { get; set; }

        [Display(Name = "نوع مددجو")]
        [Required(ErrorMessage = "وارد کردن نوع مددجو الزامی میباشد")]
        public int? ClientTypeId { get; set; }

        public string ClientTypeDescription { get; set; }
        public ClientType ClientType { get; set; }

        public bool IsHouseHold { get; set; }

        //پایان مشخصات پرونده بهزیستی


        // مشخصات تماس

        [Display(Name = "آدرس محل کار")]
        public string WorkAddress { get; set; }

        [RegularExpression("[0-9]+", ErrorMessage = "تلفن فقط شامل اعداد بین 0 تا 9 می باشد")]
        [Display(Name = "تلفن منزل")]
        public string HomeTel { get; set; }

        [RegularExpression("[0-9]+", ErrorMessage = "تلفن فقط شامل اعداد بین 0 تا 9 می باشد")]
        [Display(Name = "تلفن محل کار")]
        public string WorkTel { get; set; }

        [RegularExpression("[0-9]+", ErrorMessage = "شماره موبایل فقط شامل اعداد بین 0 تا 9 می باشد")]
        [Display(Name = "موبایل (تلفن همراه)")]
        [Required(ErrorMessage = "وارد کردن موبایل ضروری می باشد")]
        [StringLength(11, ErrorMessage = "شماره ضروری به صورت صحیح وارد نشده است", MinimumLength = 11)]
        public string Mobile { get; set; }

        [RegularExpression("[0-9]+", ErrorMessage = "شماره موبایل فقط شامل اعداد بین 0 تا 9 می باشد")]
        [Display(Name = "موبایل جهت تماس های ضروری")]
        [Required(ErrorMessage = "وارد کردن تلفن اضطراری ضروری می باشد")]
        [StringLength(11, ErrorMessage = "شماره ضروری به صورت صحیح وارد نشده است", MinimumLength = 11)]
        public string EmergencyTel { get; set; }

        //پایان مشخصات تماس


        // اطلاعات مسکن فعلی


        [Display(Name = "شهر یا روستا")]
        [Required(ErrorMessage = "انتخاب شهر ضروری می باشد")]
        public int HouseCityId { get; set; }
        public City HouseCity { get; set; }

        [Display(Name = "آدرس کامل")]
        [Required(ErrorMessage = "آدرس کامل ضروری می باشد")]
        public string Address { get; set; }

        // محدوده استان فارس بر اساس طول و عرض جغرافیایی
        // البته این محدوده به صورت مستطیلی در نظر گرفته شده و خطا دارد

        [Display(Name = "عرض جغرافیایی")]
        [Range(27.04292029822223, 31.67018422677551, ErrorMessage = "عرض جغرافیایی وارد شده در محدوده استان فارس نمی باشد")]
        public double Latitude { get; set; }
        [Display(Name = "طول جغرافیایی")]
        [Range(50.602684020996094, 55.58086395263672, ErrorMessage = "طول جغرافیایی وارد شده در محدوده استان فارس نمی باشد")]
        public double Longitude { get; set; }

        [Display(Name = "کد پستی")]
        [MinLength(11, ErrorMessage = "کد پستی باید 11 رقم باشد")]
        [MaxLength(11, ErrorMessage = "کد پستی باید 11 رقم باشد")]
        public string PostalCode { get; set; }

        [Display(Name = "وضعیت سکونت")]
        [Required(ErrorMessage = "انتخاب وضعیت سکونت ضروری می باشد")]
        public int CurrentHouseTypeId { get; set; }
        public CurrentHouseType CurrentHouseType { get; set; }

        [Display(Name = "میزان ودیعه")]
        public long? DepositAmount { get; set; }

        [Display(Name = "میزان اجاره ماهانه")]
        public long? MonthlyRentalRate { get; set; }

        [Display(Name = "نام فردی که مددجو در خانه او ساکن است")]
        public string RelativeFamilyNameWhoClientLiveInHerHouse { get; set; }

        [Display(Name = "توضیحات بیشتر")]
        public string OtherDescription { get; set; }

        //پایان اطلاعات مسکن فعلی


        public ICollection<ClientWaitingApplicantRequest> Requests { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// در صورتی که متقاضی تایید نشده بود این گزینه به معنی حذف شده می باشد
        /// </summary>
        public bool IsDeleted { get; set; }

        public ICollection<ClientWaitingApplicantLog> ClientWaitingApplicantLogs { get; set; }

    }
}