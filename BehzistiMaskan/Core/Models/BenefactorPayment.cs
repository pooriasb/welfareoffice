using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class BenefactorPayment
    {
        public int Id { get; set; }

        public int BenefactorId { get; set; }
        public Benefactor Benefactor { get; set; }

        // تاریخ پرداخت
        public DateTime PayDateTime { get; set; }

        // مبلغ پرداخت
        public double Amount { get; set; }

        // نیت از پرداخت -- تمایل دارد در چه زمینه ای خرچ شود
        public string Purpose { get; set; }

        // توضیحات پرداخت
        public string Description { get; set; }
    }
}