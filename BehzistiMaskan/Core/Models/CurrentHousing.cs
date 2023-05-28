using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class CurrentHousing
    {
        public CurrentHousing()
        {
            DepositAmount = 0;
            MonthlyRentalRate = 0;
        }
        public int Id { get; set; }

        public Client Client { get; set; }

        [Display(Name = "شهر یا روستا")]
        [Required(ErrorMessage = "انتخاب شهر ضروری می باشد")]
        public int CityId { get; set; }
        public City City { get; set; }

        [Display(Name = "آدرس مسکن")]
        [Required(ErrorMessage = "آدرس مسکن ضروری می باشد")]
        public string Address { get; set; }

        [Display(Name = "آدرس مسکن فعلی")]
        [Required(ErrorMessage = "آدرس مسکن فعلی ضروری می باشد")]
        public string AddressCurrentHouse { get; set; }

        // محدوده استان فارس بر اساس طول و عرض جغرافیایی
        // البته این محدوده به صورت مستطیلی در نظر گرفته شده و خطا دارد

        [Display(Name = "عرض جغرافیایی")]
        [Range(27.04292029822223, 31.67018422677551, ErrorMessage = "عرض جغرافیایی وارد شده در محدوده استان فارس نمی باشد")]
        public double? Latitude { get; set; }
        [Display(Name = "طول جغرافیایی")]
        [Range(50.602684020996094, 55.58086395263672, ErrorMessage = "طول جغرافیایی وارد شده در محدوده استان فارس نمی باشد")]
        public double? Longitude { get; set; }

        [Display(Name = "کد پستی")]
        [MinLength(10, ErrorMessage = "کد پستی باید 10 رقم باشد")]
        [MaxLength(10, ErrorMessage = "کد پستی باید 10 رقم باشد")]
        public string PostalCode { get; set; }

        [Display(Name = "وضعیت سکونت")]
        //[Required(ErrorMessage = "انتخاب وضعیت سکونت ضروری می باشد")]
        public int? CurrentHouseTypeId { get; set; }
        public CurrentHouseType CurrentHouseType { get; set; }

        [Display(Name = "میزان ودیعه")]
        public long? DepositAmount { get; set; }

        [Display(Name = "میزان اجاره ماهانه")]
        public long? MonthlyRentalRate { get; set; }

        [Display(Name = "اسکن قولنامه منزل")]
        public int? HomeContractDocumentId { get; set; }
        public ClientDocument HomeContract { get; set; }

        [Display(Name = "مدرک اثبات")]
        public int? ProvingBenefactorDocumentId { get; set; }
        public ClientDocument ProvingBenefactor { get; set; }

        [Display(Name = "نام فردی که مددجو در خانه او ساکن است")]
        public string RelativeFamilyNameWhoClientLiveInHerHouse { get; set; }

        [Display(Name = "توضیحات بیشتر")]
        public string OtherDescription { get; set; }

    }
}