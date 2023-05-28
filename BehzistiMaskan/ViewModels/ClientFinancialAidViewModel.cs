using System;
using System.Collections.Generic;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Utility;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.ViewModels
{
    public class ClientFinancialAidViewModel
    {
        public int ClientId { get; set; }

        public ClientDto Client { get; set; }

        public bool IsRegisteredInAnyForm { get; set; }

        public long FinancialAidAmountThatClientMustGet { get; set; }

        public FinancialAid FinancialAid { get; set; }
        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "وارد کردن مبلغ ضروری می باشد")]
        public string  Amount { get; set; }
        [Display(Name = "تاریخ واریز وجه")]
        [Required(ErrorMessage = "وارد کردن تاریخ واریز ضروری می باشد")]
        public string DepositDate { get; set; }

        public IEnumerable<CoOrganizationType> AllCoOrganizationTypes { get; set; }

        public IEnumerable<FinancialAid> ClientFinancialAids { get; set; }

        public long FinancialAidSummation { get; set; }
    }
}