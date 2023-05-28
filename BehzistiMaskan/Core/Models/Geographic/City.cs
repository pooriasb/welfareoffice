
using System;
using System.Collections.Generic;

namespace BehzistiMaskan.Core.Models.Geographic
{
    public class City
    {
        public City()
        {
            HousesInThisCity = new HashSet<CurrentHousing>();

            PersonsWhoBirthIsInThisCity = new HashSet<Person>();

            ClientWaitingWhoBirthInThisCity = new HashSet<ClientWaitingApplicant>();
        }

        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public int DistrictId { get; set; }
        public District District { get; set; }

        public bool IsVillage { get; set; }

        public ICollection<Person> PersonsWhoBirthIsInThisCity { get; set; }

        public ICollection<CurrentHousing> HousesInThisCity { get; set; }

        public string Dehestan { get; set; }

        public ICollection<Client> Clients { get; set; }

        public ICollection<ClientWaitingApplicant> ClientWaitingWhoBirthInThisCity { get; set; }
        public ICollection<ClientWaitingApplicant> ClientWaitingWhomBehzistiDocumentInThisCity { get; set; }
        public ICollection<ClientWaitingApplicant> ClientWaitingWhoTheirHouseIsInThisCity { get; set; }
    }
}