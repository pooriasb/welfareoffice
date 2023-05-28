using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientWaitingApplicantLogConfiguration : EntityTypeConfiguration<ClientWaitingApplicantLog>
    {
        public ClientWaitingApplicantLogConfiguration()
        {
            HasRequired(l => l.ClientWaitingApplicant)
                .WithMany(w => w.ClientWaitingApplicantLogs)
                .HasForeignKey(l => l.ClientWaitingApplicantId);


        }
    }
}