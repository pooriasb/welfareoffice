using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.ReportBuilder;

namespace BehzistiMaskan.ViewModels
{
    public class ReportDesignerViewModel
    {
        public Report Report { get; set; }


        [Display(Name = "لیست همه شهرستان ها")]
        public IEnumerable<CountyDto> AllCounties { get; set; }

        [Display(Name = "شهرستان های انتخاب شده")]
        public IEnumerable<int> SelectedBehzistiDocumentCounties { get; set; }

        public IEnumerable<FormDto> AllForms { get; set; }

        public string JsonSelectedFormFields { get; set; }

    }
}