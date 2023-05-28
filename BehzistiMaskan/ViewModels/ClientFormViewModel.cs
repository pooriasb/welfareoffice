using System;
using System.Collections.Generic;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class ClientFormViewModel
    {
        public IEnumerable<GenderType> GenderTypes { get; set; }
        public IEnumerable<ClientType> ClientTypes { get; set; }
        public IEnumerable<MarriageType> MarriageTypes { get; set; }
        public IEnumerable<AssistanceType> AssistanceTypes { get; set; }

        public IEnumerable<CityDto> Cities { get; set; }
        public IEnumerable<DistrictDto> Districts { get; set; }
        public IEnumerable<CountyDto> Counties { get; set; }
        public IEnumerable<ProvinceDto> Provinces { get; set; }

        public IEnumerable<CityDto> CityOfBirth { get; set; }
        public IEnumerable<DistrictDto> DistrictOfBirth { get; set; }
        public IEnumerable<CountyDto> CountyOfBirth { get; set; }
        public IEnumerable<ProvinceDto> ProvinceOfBirth { get; set; }

        public ClientDto Client { get; set; }
        public Status Status { get; set; }

    }
}