using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class FinancialAidConfiguration:EntityTypeConfiguration<FinancialAid>
    {
        public FinancialAidConfiguration()
        {
            //Table Name

            //Key
            HasKey(c => c.Id);

            //Property

            //Relations
            HasRequired(f => f.Client)
                .WithMany(c => c.FinancialAids)
                .HasForeignKey(c=>c.ClientId)
                .WillCascadeOnDelete(false);

            HasOptional(f=>f.ClientDocument)
                .WithMany(cd=>cd.FinancialAids)
                .HasForeignKey(f=>f.ClientDocumentId)
                .WillCascadeOnDelete(false);

        }
    }
}