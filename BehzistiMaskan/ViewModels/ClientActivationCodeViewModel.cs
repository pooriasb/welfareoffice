using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.ViewModels
{
    public class ClientActivationCodeViewModel
    {
        public int ClientId { get; set; }

        public string NationalCode { get; set; }

        public string Name { get; set; }

        public string Family { get; set; }

        public string ActivationCode { get; set; }
    }
}