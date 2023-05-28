using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BehzistiMaskan.Core.Models.Utility
{
    public class CurrentHouseType
    {
        public CurrentHouseType()
        {
            CurrentHousings = new HashSet<CurrentHousing>();
        }
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public ICollection<CurrentHousing> CurrentHousings { get; set; }

        public ICollection<ClientWaitingApplicant> ClientWaitingLists { get; set; }

    }
}