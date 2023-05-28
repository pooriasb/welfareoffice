using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;
using MD.PersianDateTime;

namespace BehzistiMaskan.Core.Dtos
{
    public class BenefactorDto
    {
        
        public int Id { get; set; }

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "کد ملی ضروری می باشد")]
        [MaxLength(10, ErrorMessage = "کد ملی به صورت صحیح وارد نشده است")]
        [MinLength(10, ErrorMessage = "کد ملی به صورت صحیح وارد نشده است")]
        public string NationalCode { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "نام ضروری می باشد")]
        public string Name { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "نام خانوادگی ضروری می باشد")]
        public string Family { get; set; }

        [Display(Name = "نام پدر")]
        public string FatherName { get; set; }

        [Display(Name = "تاریخ تولد")]
        public PersianDateTime? Birthdate { get; set; }

        [Display(Name = "جنسیت")]
        public int? GenderTypeId { get; set; }
        public GenderType GenderType { get; set; }

        [Display(Name = "وضعیت تاهل")]
        public int? MarriageTypeId { get; set; }
        public MarriageType MarriageType { get; set; }

        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "شماره موبایل ضروری می باشد")]
        [MaxLength(11, ErrorMessage = "موبایل به صورت صحیح وارد نشده است")]
        [MinLength(11, ErrorMessage = "موبایل به صورت صحیح وارد نشده است")]
        public string Mobile { get; set; }

        public ICollection<BenefactorPayment> BenefactorPayments { get; set; }

        // کمک به صورت نقدی
        public bool WillHelpWithCash{ get; set; }
        public bool WillHelpWithGift{ get; set; }
        public bool WillHelpWithService { get; set; }

        // کمک به ساخت خانه
        public bool HelpToCreateAHouse { get; set; }
        // کمک به خرید خانه
        public bool HelpToBuyAHouse { get; set; }
        // کمک به تعمیر خانه
        public bool HelpToFixAHouse{ get; set; }
        // کمک به پرداخت اجاره ماهانه
        public bool HelpToPayMonthlyRental{ get; set; }
        // کمک به پرداخت ودیعه مسکن
        public bool HelpToPayMortgageMoney{ get; set; }
        // کمک به پرداخت قسط وام مسکن
        public bool HelpToPayLoanQuarter{ get; set; }

        public bool HelpWithOtherMethod { get; set; }

        // وضعیت همکاری -- آیا همکاری به صورت مستمر می باشد
        [Display(Name = "همکاری به صورت مستمر")]
        public bool IsContinuum { get; set; }

        [Display(Name = "نحوه ارائه کمک")]
        public bool WantOnlinePayment { get; set; }

        [Display(Name = "میزان کمک (به ریال)")]
        public long? HelpAmount { get; set; }

        // نحوه همکاری مد نظر خیر
        [Display(Name="توضیح سایر زمینه ها")]
        public string Description { get; set; }

        [Display(Name = "شهرستان")]
        public int? CountyId { get; set; }
        public County County { get; set; }

        [Display(Name = "استان")]
        public int ProvinceId { get; set; }
        public Province Province { get; set; }
    }
}