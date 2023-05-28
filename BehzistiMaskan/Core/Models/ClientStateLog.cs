using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class ClientStateLog
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

        public ModelEnums.ClientStateTypeE ClientStateTypeE { get; set; }

        public DateTime LogDate { get; set; }


        public string Description { get; set; }
    }
}