using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    /// <summary>
    /// کلاس برای ذخیره اطلاعات بهره مندی مددجو از معافیت انشعابات
    /// </summary>
    public class ClientExemptionBenefit
    {
        public int Id { get; set; }

        public int ClientRequestId { get; set; }
        public ClientRequest ClientRequest { get; set; }

        /// <summary>
        /// تاریخ بهره مندی از معافیت
        /// </summary>
        public DateTime BenefitDate { get; set; }

        /// <summary>
        /// میزان معافیت - مبلغ به ریال
        /// </summary>
        public long BenefitAmount { get; set; }


        public int BenefitPhotoId { get; set; }
        public ClientDocument BenefitPhoto { get; set; }
    }
}