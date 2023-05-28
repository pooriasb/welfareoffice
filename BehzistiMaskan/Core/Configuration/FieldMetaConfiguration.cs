﻿using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Configuration
{
    public class FieldMetaConfiguration : EntityTypeConfiguration<FieldMeta>
    {
        public FieldMetaConfiguration()
        {
            //Table Name
            ToTable("FieldMeta");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(f => f.Key)
                .HasMaxLength(200)
                .IsRequired();

            Property(f => f.Value)
                .HasMaxLength(2000)
                .IsRequired();


            //relation between Field and FieldMeta are declared in FieldConfiguration.cs
        }
    }
}