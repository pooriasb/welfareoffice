
using System.Collections.Generic;

namespace BehzistiMaskan.ViewModels
{
    public class DashboardViewModel
    {
        public int ClientCount { get; set; }

        //تعداد افرادی که در گزارش جامع درج شده اند

        public int ClientExecutedCount { get; set; }

        // تعداد افرادی که در گزارش جامع هستند + افرادی که حذف شده اند + افرادی که متقاضی معافیت هستند
        public int ClientActionCount { get; set; }

        public int ClientWaitingApplicantCount { get; set; }

        public IEnumerable<DashboardFormSimpleData> DashboardFormSimpleDatas { get; set; }

        public IEnumerable<DashboardFormDataByPhysicalProgress> DashboardFormDataByPhysicalProgresses { get; set; }

        public IEnumerable<DashboardFormSimpleData> DashboardFormByMoney { get; set; }

        public IEnumerable<DashboardExemptionSimpleData> DashboardExemptionSimpleDataThisYear { get; set; }
        public IEnumerable<DashboardExemptionSimpleData> DashboardExemptionSimpleDataLastYear { get; set; }

        /// <summary>
        /// تعداد کل متقاضیان معافیت انشعابات
        /// </summary>
        public int NumberOfPersonWhoRequestExemptionThisYear { get; set; }
        /// <summary>
        /// تعداد افرادی که از معافیت انشعابات بهره مند شده اند
        /// </summary>
        public int NumberOfPersonWhoBenefitedFromExemptionThisYear { get; set; }
        /// <summary>
        /// مبلغ کل بهره مندی از معافیت انشعابات
        /// </summary>
        public long AmountOfAllExemptionBenefitThisYear { get; set; }
        /// <summary>
        /// تعداد کل متقاضیان معافیت سال گذشته
        /// </summary>
        public int NumberOfPersonWhoRequestExemptionLastYear { get; set; }
        /// <summary>
        ///  تعداد کل افراد بهره مند شده از معافیت در سال گذشته
        /// </summary>
        public int NumberOfPersonWhoBenefitedFromExemptionLastYear { get; set; }
        /// <summary>
        /// میزان بهره مندی از معافیت در سال گذشته
        /// </summary>
        public long AmountOfAllExemptionBenefitLastYear { get; set; }

    }
}