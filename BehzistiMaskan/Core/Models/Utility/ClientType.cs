using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BehzistiMaskan.Core.Models.Utility
{
    public class ClientType
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public ICollection<Client> Clients { get; set; }
        public ICollection<ClientWaitingApplicant> ClientWaitingLists { get; set; }
    }
}