
namespace BehzistiMaskan.ViewModels
{
    public class DashboardExemptionSimpleData
    {
        /// <summary>
        /// نام معافیت
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// تعداد افراد متقاضی این نوع معافیت
        /// </summary>
        public int RequestedNumber { get; set; }
        /// <summary>
        /// تعداد افراد بهره مند شده
        /// </summary>
        public int BenefitedNumber { get; set; }
        /// <summary>
        /// میزان و مبلغ بهره مندی
        /// </summary>
        public long BenefitAmount { get; set; }
    }
}