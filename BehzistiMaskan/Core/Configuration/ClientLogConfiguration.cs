using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientLogConfiguration:EntityTypeConfiguration<ClientLog>
    {
        public ClientLogConfiguration()
        {

            Property(c => c.Detail)
                .IsRequired();

            HasRequired(cl=>cl.Client)
                .WithMany(c=>c.ClientLogs)
                .HasForeignKey(cl=>cl.ClientId)
                .WillCascadeOnDelete(false);

        }
    }
}