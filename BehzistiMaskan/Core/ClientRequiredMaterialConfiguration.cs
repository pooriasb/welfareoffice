using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core
{
    public class ClientRequiredMaterialConfiguration : EntityTypeConfiguration<ClientRequiredMaterial>
    {
        public ClientRequiredMaterialConfiguration()
        {
            HasKey(rm => rm.Id);

            Property(rm => rm.Date)
                .IsRequired();

            Property(rm => rm.Description)
                .IsMaxLength()
                .IsRequired();


            //Relation
            HasRequired(rm => rm.Client)
                .WithMany(c => c.ClientRequiredMaterials)
                .HasForeignKey(rm => rm.ClientId);

            HasRequired(rm => rm.MaterialType)
                .WithMany(mt => mt.RequiredMaterialsWithThisType)
                .HasForeignKey(rm => rm.MaterialTypeId);
        }
    }
}