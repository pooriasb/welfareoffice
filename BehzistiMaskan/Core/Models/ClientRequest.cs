using System.Collections.Generic;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class ClientRequest
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }


        public int RequestTypeId { get; set; }
        public RequestType RequestType { get; set; }

        public ICollection<ClientRequestGetLetter> GetLetters { get; set; }
        public ICollection<ClientExemptionBenefit> ExemptionBenefits { get; set; }
    }
}