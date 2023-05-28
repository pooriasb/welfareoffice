using System.Collections.Generic;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Utility;
using MD.PersianDateTime;
using System.ComponentModel.DataAnnotations;

namespace BehzistiMaskan.ViewModels
{
    public class BenefactorViewModel
    {
        public BenefactorDto Benefactor { get; set; }

        [Display(Name = "تاریخ تولد")]
        public string Birthdate { get; set; }
        public IEnumerable<MarriageType> MarriageTypes { get; set; }
        public IEnumerable<GenderType> GenderTypes { get; set; }

        public IEnumerable<ProvinceDto> Provinces { get; set; }
        public IEnumerable<CountyDto> Counties { get; set; }

    }
}