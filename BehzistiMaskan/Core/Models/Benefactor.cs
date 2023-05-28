using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class Benefactor
    {
        public int Id { get; set; }

        [Display(Name="کد ملی")]
        public string NationalCode { get; set; }

        public string Name { get; set; }

        public string Family { get; set; }

        public string FatherName { get; set; }

        public DateTime? Birthdate { get; set; }

        public int? GenderTypeId { get; set; }
        public GenderType GenderType { get; set; }

        public int? MarriageTypeId { get; set; }
        public MarriageType MarriageType { get; set; }

        [Display(Name="شماره موبایل")]
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
        public bool IsContinuum { get; set; }

        public bool WantOnlinePayment { get; set; }

        public double? HelpAmount { get; set; }

        // نحوه همکاری مد نظر خیر
        public string Description { get; set; }

        public int? CountyId { get; set; }
        public County County { get; set; }

        public int ProvinceId { get; set; }
        public Province Province { get; set; }
    }
}