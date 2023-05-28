using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class DeletedClient
    {
        public int Id { get; set; }

        public Client Client { get; set; }

        public string ReasonDescription { get; set; }

        public DateTime DeleteDateTime { get; set; }
    }
}