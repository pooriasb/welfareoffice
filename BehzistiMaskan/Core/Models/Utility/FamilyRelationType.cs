using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BehzistiMaskan.Core.Models.Utility
{
    public class FamilyRelationType
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<FamilyRelation> FamilyRelations { get; set; }
    }
}