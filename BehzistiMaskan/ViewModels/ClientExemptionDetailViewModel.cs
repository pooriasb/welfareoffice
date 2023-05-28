using System.Collections.Generic;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class ClientExemptionDetailViewModel
    {
        public RequestType RequestType { get; set; }

        public IEnumerable<ClientRequestGetLetter> GetLetters { get; set; }
        public IEnumerable<ClientExemptionBenefit> ExemptionBenefits { get; set; }

        /// <summary>
        /// آیا تقاضای معافیت داده است
        /// </summary>
        public bool HasRequestExemption { get; set; }
        /// <summary>
        /// آیا نامه معرفی را دریافت کرده است
        /// </summary>
        public bool HasGetExemptionLetter { get; set; }
        /// <summary>
        /// آیا بهره مند شده است
        /// </summary>
        public bool HasBenefitedFromExemption { get; set; }
    }
}