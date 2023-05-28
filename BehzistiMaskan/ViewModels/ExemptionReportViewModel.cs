using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.Utility;
using MD.PersianDateTime;

namespace BehzistiMaskan.ViewModels
{
    public class ExemptionReportViewModel
    {
        [Display(Name = "نوع معافیت")]
        public int? SelectedExemptionTypeId { get; set; }

        public IEnumerable<RequestType> AllExemptionRequests { get; set; }

        [Display(Name = "فقط افرادی که بهره مند شده اند")]
        public bool HasBenefited { get; set; }

        [Display(Name = "از تاریخ:")]
        [DataType(DataType.Date)]
        public  PersianDateTime? GetLetterStartDateRange { get; set; }
        [Display(Name = "تا تاریخ:")]
        [DataType(DataType.Date)]
        public PersianDateTime? GetLetterEndDateRange { get; set; }

        [Display(Name = "از تاریخ:")]
        [DataType(DataType.Date)]
        public PersianDateTime? BenefitStartDateRange { get; set; }
        [Display(Name = "تا تاریخ:")]
        [DataType(DataType.Date)]
        public PersianDateTime? BenefitEndDateRange { get; set; }


        public IEnumerable<ExemptionReportSimpleDto> ExemptionReportResult { get; set; }
    }
}