using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Utility;
using Newtonsoft.Json;

namespace BehzistiMaskan.Core.Models
{
    public class ClientState
    {
        public int Id { get; set; }

        [JsonIgnore]
        public Client Client { get; set; }

        public ModelEnums.ClientStateTypeE ClientStateTypeE { get; set; }

        public DateTime CurrentStateDate { get; set; }

        public DateTime PrevStateDate { get; set; }

    }
}