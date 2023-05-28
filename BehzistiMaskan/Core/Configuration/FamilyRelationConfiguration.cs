using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class FamilyRelationConfiguration : EntityTypeConfiguration<FamilyRelation>
    {
        public FamilyRelationConfiguration()
        {
            //Table Name
            ToTable("FamilyRelation");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(f => f.Description)
                .HasMaxLength(2000);

            //Relation
            HasRequired(f => f.FamilyRelationType)
                .WithMany(f => f.FamilyRelations)
                .HasForeignKey(f => f.FamilyRelationTypeId);
        }
    }
}