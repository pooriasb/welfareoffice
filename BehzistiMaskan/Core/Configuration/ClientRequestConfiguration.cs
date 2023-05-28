using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientRequestConfiguration : EntityTypeConfiguration<ClientRequest>
    {
        public ClientRequestConfiguration()
        {
            //Table Name
            ToTable("ClientRequest");

            //Key
            HasKey(c => c.Id);

            //Property


            //Relations
            HasRequired(r=>r.Client)
                .WithMany(c=>c.ClientRequests)
                .HasForeignKey(r=>r.ClientId)
                .WillCascadeOnDelete(false);

            HasRequired(r=>r.RequestType)
                .WithMany(r=>r.ClientRequests)
                .HasForeignKey(r=>r.RequestTypeId)
                .WillCascadeOnDelete(false);


        }
    }
}