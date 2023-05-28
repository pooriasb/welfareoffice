using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientWaitingApplicantRequestConfiguration :EntityTypeConfiguration<ClientWaitingApplicantRequest>
    {
        public ClientWaitingApplicantRequestConfiguration()
        {
            //Table Name
            ToTable("ClientWaitingApplicantRequest");

            //Key
            HasKey(c => c.Id);

            //Property
            

            //Relations
            HasRequired(r=>r.ClientWaitingApplicant)
                .WithMany(c=>c.Requests)
                .HasForeignKey(r=>r.ClientWaitingApplicantId)
                .WillCascadeOnDelete(true);

            HasRequired(r=>r.RequestType)
                .WithMany(r=>r.ClientWaitingApplicantRequests)
                .HasForeignKey(r=>r.RequestTypeId)
                .WillCascadeOnDelete(false);
        }
    }
}