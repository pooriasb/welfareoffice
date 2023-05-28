using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientExemptionBenefitConfiguration : EntityTypeConfiguration<ClientExemptionBenefit>
    {
        public ClientExemptionBenefitConfiguration()
        {
            //Table Name

            // Key
            HasKey(eb => eb.Id);

            //Property

            //Relations
            HasRequired(eb => eb.ClientRequest)
                .WithMany(cr => cr.ExemptionBenefits)
                .HasForeignKey(eb => eb.ClientRequestId)
                .WillCascadeOnDelete(false);

            HasRequired(eb=>eb.BenefitPhoto)
                .WithMany(cd=>cd.ClientExemptionBenefits)
                .HasForeignKey(eb=>eb.BenefitPhotoId)
                .WillCascadeOnDelete(false);
        }
    }
}