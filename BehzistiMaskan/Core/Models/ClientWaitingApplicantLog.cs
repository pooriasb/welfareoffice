using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class ClientWaitingApplicantLog
    {
        public int Id { get; set; }

        public int ClientWaitingApplicantId { get; set; }

        public ClientWaitingApplicant ClientWaitingApplicant { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Description { get; set; }
    }
}