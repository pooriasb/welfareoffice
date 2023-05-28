using System.Collections.Generic;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class ClientRequestViewModel
    {

        public IEnumerable<RequestType> RequestTypes { get; set; }

        public int ClientId { get; set; }
        public ClientDto Client { get; set; }
        public Status Status { get; set; }

        public int RequestTypeBuildingId { get; set; }

        public bool HasRequestWaterExemption { get; set; }
        public bool HasBenefitWaterExemption { get; set; }

        public bool HasRequestGasExemption { get; set; }
        public bool HasBenefitGasExemption { get; set; }

        public bool HasRequestElectricalExemption { get; set; }
        public bool HasBenefitElectricalExemption { get; set; }

        public bool HasRequestProductionLicenseExemption { get; set; }
        public bool HasBenefitProductionLicenseExemption { get; set; }

        /// <summary>
        /// در صورتی که از کمک هزینه ساخت مسکن استفاده کرده بود و یا در انتظار پرداخت بود
        /// </summary>
        public bool HasBenefitFromBuildingAid { get; set; }
    }
}