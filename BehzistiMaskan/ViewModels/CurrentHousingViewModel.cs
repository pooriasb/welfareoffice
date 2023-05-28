using System.Collections.Generic;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class CurrentHousingViewModel
    {
        public CurrentHousing CurrentHousing { get; set; }

        public IEnumerable<CountyDto> CountyOfCurrentHousing { get; set; }
        public IEnumerable<DistrictDto> DistrictOfCurrentHousing { get; set; }
        public IEnumerable<CityDto> CityOfCurrentHousing { get; set; }
        public IEnumerable<CurrentHouseType> CurrentHouseTypes { get; set; }

        public int ClientId { get; set; }
        public ClientDto Client { get; set; }
        public Status Status { get; set; }

    }
}