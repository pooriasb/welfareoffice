using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Dtos
{
    public class ExemptionReportSimpleDto
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string NationalCode { get; set; }
        public string CountyName { get; set; }
        public string Address { get; set; }
        
        public bool HasGetLetterWater { get; set; }
        public string GetLetterDateWater { get; set; }
        public string GetLetterNumberWater { get; set; }
        public bool HasBenefitedWater { get; set; }
        public string BenefitDateWater { get; set; }
        public long BenefitAmountWater { get; set; }

        public bool HasGetLetterElectrical { get; set; }
        public string GetLetterDateElectrical { get; set; }
        public string GetLetterNumberElectrical { get; set; }
        public bool HasBenefitedElectrical { get; set; }
        public string BenefitDateElectrical { get; set; }
        public long BenefitAmountElectrical { get; set; }


        public bool HasGetLetterGas { get; set; }
        public string GetLetterDateGas { get; set; }
        public string GetLetterNumberGas { get; set; }
        public bool HasBenefitedGas { get; set; }
        public string BenefitDateGas { get; set; }
        public long BenefitAmountGas { get; set; }


        public bool HasGetLetterProductionLicense { get; set; }
        public string GetLetterDateProductionLicense { get; set; }
        public string GetLetterNumberProductionLicense { get; set; }
        public bool HasBenefitedProductionLicense { get; set; }
        public string BenefitDateProductionLicense { get; set; }
        public long BenefitAmountProductionLicense { get; set; }

    }
}