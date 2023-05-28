using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BehzistiMaskan.Core.Models.Utility
{
    public class GenderType
    {
        public GenderType()
        {
            Persons = new HashSet<Person>();
            Benefactors = new HashSet<Benefactor>();
            ClientWaitingLists = new HashSet<ClientWaitingApplicant>();
        }
        public int Id { get; set; }

        [StringLength(25)]
        public string Name { get; set; }

        public ICollection<Person> Persons { get; set; }
        public ICollection<Benefactor> Benefactors { get; set; }

        public ICollection<ClientWaitingApplicant> ClientWaitingLists { get; set; }

    }
}