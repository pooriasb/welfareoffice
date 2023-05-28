using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class CoOrganizationApproveList
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        public Client Client { get; set; }

        public int CoOrganizationTypeId { get; set; }
        public CoOrganizationType CoOrganizationType { get; set; }

        public DateTime ApproveDateTime { get; set; }

        public string Description { get; set; }
    }
}