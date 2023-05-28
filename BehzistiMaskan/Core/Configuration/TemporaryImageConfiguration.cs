using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class TemporaryImageConfiguration : EntityTypeConfiguration<TemporaryImage>
    {
        public TemporaryImageConfiguration()
        {
            //Table Name
            //ToTable("");

            //Key
            HasKey(c => c.Id);


            //Relations
            HasOptional(t => t.Client)
                .WithMany(c => c.TemporaryImages)
                .HasForeignKey(t=>t.ClientId);

            HasOptional(t => t.Field)
                .WithMany(c => c.TemporaryImages)
                .HasForeignKey(t=>t.FieldId);
        }
    }
}