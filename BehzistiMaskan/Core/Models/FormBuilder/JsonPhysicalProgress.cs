using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class JsonPhysicalProgress
    {
        public int PhysicalProgressId { get; set; }

        public List<JsonPhysicalProgressHelp> CoOrganizationQuotaList { get; set; }
    }
}