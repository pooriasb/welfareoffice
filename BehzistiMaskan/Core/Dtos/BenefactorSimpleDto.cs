
using System;

namespace BehzistiMaskan.Core.Dtos
{
    public class BenefactorSimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string FatherName { get; set; }

        public string Mobile { get; set; }


        public string NationalCode { get; set; }
        public string ProvinceName { get; set; }
        public string CountyName { get; set; }



        // کمک به صورت نقدی
        public string WillHelpWithCash { get; set; }
        public string WillHelpWithGift { get; set; }
        public string WillHelpWithService { get; set; }

        // کمک به ساخت خانه
        public string HelpToCreateAHouse { get; set; }
        // کمک به خرید خانه
        public string HelpToBuyAHouse { get; set; }
        // کمک به تعمیر خانه
        public string HelpToFixAHouse { get; set; }
        // کمک به پرداخت اجاره ماهانه
        public string HelpToPayMonthlyRental { get; set; }
        // کمک به پرداخت ودیعه مسکن
        public string HelpToPayMortgageMoney { get; set; }
        // کمک به پرداخت قسط وام مسکن
        public string HelpToPayLoanQuarter { get; set; }


        // وضعیت همکاری -- آیا همکاری به صورت مستمر می باشد
        public string IsContinuum { get; set; }

        public string WantOnlinePayment { get; set; }

        public double? HelpAmount { get; set; }



    }
}