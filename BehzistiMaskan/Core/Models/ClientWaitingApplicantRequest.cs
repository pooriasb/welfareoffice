using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class ClientWaitingApplicantRequest
    {
        public int Id { get; set; }

        public int ClientWaitingApplicantId { get; set; }

        public ClientWaitingApplicant ClientWaitingApplicant { get; set; }

        public int RequestTypeId { get; set; }

        public RequestType RequestType { get; set; }
    }
}