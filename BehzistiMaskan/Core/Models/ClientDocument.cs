using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class ClientDocument
    {
        public int Id { get; set; }

        public int  DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }

        public string DocURI { get; set; }

        public bool IsOptimized { get; set; }

        public Client Client { get; set; }
        public int ClientId { get; set; }

        public ICollection<CurrentHousing> CurrentHousingByHomeContract { get; set; }
        public ICollection<CurrentHousing> CurrentHousingByProvingBenefactor { get; set; }
        public ICollection<FinancialAid> FinancialAids { get; set; }
        public ICollection<BankInfo> BankInfos { get; set; }
        public ICollection<ClientRequestGetLetter> ClientRequestGetLetters { get; set; }
        public ICollection<ClientExemptionBenefit> ClientExemptionBenefits { get; set; }


        public ClientPhysicalProgressPhoto ClientPhysicalProgressPhoto { get; set; }

        public FormEmtiazBandi FormEmtiazBandi { get; set; }
    }
}