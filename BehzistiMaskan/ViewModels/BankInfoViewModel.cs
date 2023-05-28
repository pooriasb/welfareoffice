using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class BankInfoViewModel
    {
        public IEnumerable<BankType> BankTypes { get; set; }

        public BankInfo BankInfo { get; set; }

        public int ClientId { get; set; }

        public ClientDto Client { get; set; }

        public int TemporaryImageId { get; set; }
        public Status Status { get; set; }

    }
}