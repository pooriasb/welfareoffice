using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientStateLogConfiguration:EntityTypeConfiguration<ClientStateLog>
    {
        public ClientStateLogConfiguration()
        {
            HasRequired(cl=>cl.Client)
                .WithMany(c=>c.ClientStateLogs)
                .HasForeignKey(cl=>cl.ClientId)
                .WillCascadeOnDelete(false);
        }
    }
}