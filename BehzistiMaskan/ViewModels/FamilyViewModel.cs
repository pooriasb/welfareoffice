using System.Collections.Generic;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class FamilyViewModel
    {
        public int ClientId { get; set; }

        public IEnumerable<FamilyRelationDto> FamilyRelationDtos { get; set; }

        public IEnumerable<FamilyRelationType> FamilyRelationTypes { get; set; }

        public FamilyRelation FamilyRelation { get; set; }

        public ClientDto Client { get; set; }
    }
}