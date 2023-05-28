using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models.Utility
{
    public class RequestType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PersianTitle { get; set; }

        public string PersianShortTitle { get; set; }

        public bool IsExemption { get; set; }


        public ICollection<ClientWaitingApplicantRequest> ClientWaitingApplicantRequests { get; set; }

        public ICollection<ClientRequest> ClientRequests { get; set; }

        public virtual CoOrganizationType CoOrganizationType { get; set; }

    }
}