using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class CoOrganizationApproveListConfiguration:EntityTypeConfiguration<CoOrganizationApproveList>
    {
        public CoOrganizationApproveListConfiguration()
        {
            Property(ca => ca.Description)
                .IsRequired();


            HasRequired(ca=>ca.Client)
                .WithMany(co=>co.CoOrganizationApproveLists)
                .HasForeignKey(ca=>ca.ClientId)
                .WillCascadeOnDelete(false);

            HasRequired(ca=>ca.CoOrganizationType)
                .WithMany(co=>co.CoOrganizationApproveLists)
                .HasForeignKey(ca=>ca.CoOrganizationTypeId)
                .WillCascadeOnDelete(false);
        }
    }
}