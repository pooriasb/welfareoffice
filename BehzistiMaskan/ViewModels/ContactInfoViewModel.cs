using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class ContactInfoViewModel
    {
        public ContactInfo ContactInfo { get; set; }

        public int ClientId { get; set; }

        public ClientDto Client { get; set; }
        public Status Status { get; set; }
    }
}