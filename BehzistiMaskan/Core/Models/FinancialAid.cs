using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class FinancialAid
    {
        public int Id { get; set; }

        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "وارد کردن مبلغ ضروری می باشد")]
        public long Amount { get; set; }

        [Display(Name = "تاریخ واریز وجه")]
        [Required(ErrorMessage = "وارد کردن تاریخ واریز ضروری می باشد")]
        public DateTime DepositDate { get; set; }

        [Display(Name="واریز کننده")]
        public int? CoOrganizationTypeId { get; set; }
        public CoOrganizationType CoOrganizationType { get; set; }

        [Display(Name = "فیش واریزی")]
        //[Required(ErrorMessage = "آپلود تصویر فیش واریزی ضروری می باشد")]
    
        public int? ClientDocumentId { get; set; }
        public ClientDocument ClientDocument { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

    }
}