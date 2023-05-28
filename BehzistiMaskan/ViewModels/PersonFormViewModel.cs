using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class PersonFormViewModel
    {
        public PersonDto Person { get; set; }

        public IEnumerable<GenderType> GenderTypes { get; set; }
        public IEnumerable<MarriageType> MarriageTypes { get; set; }

        public IEnumerable<CityDto> CityOfBirth { get; set; }
        public IEnumerable<DistrictDto> DistrictOfBirth { get; set; }
        public IEnumerable<CountyDto> CountyOfBirth { get; set; }
        public IEnumerable<ProvinceDto> ProvinceOfBirth { get; set; }
    }
}