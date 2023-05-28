using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.ViewModels
{
    public class NewClientViewModel
    {
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }
        [Display(Name = "نام")]
        public string Name { get; set; }
        [Display(Name = "نام خانوادگی")]
        public string Family { get; set; }
        public string CityId { get; set; }
    }
}