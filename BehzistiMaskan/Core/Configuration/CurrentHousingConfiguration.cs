using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class CurrentHousingConfiguration : EntityTypeConfiguration<CurrentHousing>
    {
        public CurrentHousingConfiguration()
        {
            //Table Name
            ToTable("CurrentHousing");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(1000);

            Property(c => c.PostalCode)
                .HasMaxLength(11)
                .IsFixedLength();

            //Relation
            HasRequired(c => c.City)
                .WithMany(c => c.HousesInThisCity)
                .HasForeignKey(c => c.CityId)
                .WillCascadeOnDelete(false);

            HasOptional(c => c.HomeContract)
                .WithMany(d => d.CurrentHousingByHomeContract)
                .HasForeignKey(c => c.HomeContractDocumentId);

            HasOptional(c => c.ProvingBenefactor)
                            .WithMany(d => d.CurrentHousingByProvingBenefactor)
                            .HasForeignKey(c => c.ProvingBenefactorDocumentId);

            /*
             relation between --CurrentHousing-- and --Client-- is declared on ClientConfiguration
             */
        }
    }
}