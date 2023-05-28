using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class MaterialTypeConfiguration : EntityTypeConfiguration<MaterialType>
    {
        public MaterialTypeConfiguration()
        {

            // Key
            HasKey(m => m.Id);

            // Property

            Property(m => m.Name)
                .HasMaxLength(255)
                .IsRequired();

            Property(m => m.PersianName)
                .HasMaxLength(255)
                .IsRequired();


            //Relation

        }
    }
}