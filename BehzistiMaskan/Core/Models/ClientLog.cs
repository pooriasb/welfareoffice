using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class ClientLog
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

        public string Detail { get; set; }

        public DateTime LogDate { get; set; }
    }
}