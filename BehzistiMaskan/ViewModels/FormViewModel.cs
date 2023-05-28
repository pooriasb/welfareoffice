using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class FormViewModel
    {
        public FormDto Form { get; set; }

        public string JsonFormFieldStr { get; set; }

        [Display(Name = "همه سازمان های همکار")]
        public IEnumerable<CoOrganizationType> AllCoOrganizationTypes { get; set; }

        [Display(Name = "سازمان های همکار در این طرح")]
        public List<int> CoOrganizationTypeIds { get; set; }
        public IEnumerable<CoOrganizationType> CoOrganizationTypes { get; set; }

        [Display(Name = "شهرستان هایی که طرح در آنها اجرا می شود")]
        [Required(ErrorMessage = "انتخاب شهرستان ها ضروری می باشد")]
        public List<int> FormCountyAccessLevelIds { get; set; }

        public List<int> FormCountyQuotas { get; set; }

        public IEnumerable<FormAccessLevel> FormCountyAccessLevels { get; set; }

        [Display(Name = "لیست همه شهرستان ها")]
        public IEnumerable<CountyDto> Counties { get; set; }

        public IEnumerable<FieldTemplate> FieldTemplates { get; set; }

        public IEnumerable<PhysicalProgress> AllPhysicalProgress { get; set; }

        public string JsonFormPhysicalProgressStr { get; set; }

        public List<FormPhysicalProgress> FormPhysicalProgresses { get; set; }
    }
}