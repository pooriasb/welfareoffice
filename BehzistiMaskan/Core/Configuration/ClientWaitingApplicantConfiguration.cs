using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientWaitingApplicantConfiguration : EntityTypeConfiguration<ClientWaitingApplicant>
    {
        public ClientWaitingApplicantConfiguration()
        {
            // Table
            ToTable("ClientWaitingApplicant");

            //Property
            HasKey(c => c.Id);

            Property(p=>p.BirthCertificateNo)
                .IsRequired();
                

            Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(250);

            Property(p => p.Family)
                .IsRequired()
                .HasMaxLength(250);

            Property(p => p.FatherName)
                .IsRequired()
                .HasMaxLength(250);

            Property(p => p.MotherName)
                .IsRequired()
                .HasMaxLength(250);

            Property(p => p.NationalCode)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnAnnotation(
                    "IndexNationalCode",
                    new IndexAnnotation(new IndexAttribute("IX_NationalCode") { IsUnique = true }));

            Property(c => c.Address)
                .IsRequired()
                .HasMaxLength(1000);

            Property(c => c.PostalCode)
                .HasMaxLength(10)
                .IsFixedLength();

            Property(p=>p.Mobile)
                .HasMaxLength(11)
                .IsRequired();

            
            Property(p=>p.EmergencyTel)
                .HasMaxLength(11)
                .IsRequired();


            //Relation
            HasRequired(c => c.GenderType)
                .WithMany(g => g.ClientWaitingLists)
                .HasForeignKey(c=>c.GenderTypeId)
                .WillCascadeOnDelete(false);


            HasRequired(c => c.MarriageType)
                .WithMany(g => g.ClientWaitingLists)
                .HasForeignKey(c=>c.MarriageTypeId)
                .WillCascadeOnDelete(false);


            HasRequired(c => c.CityOfBirth)
                .WithMany(g => g.ClientWaitingWhoBirthInThisCity)
                .HasForeignKey(c=>c.CityOfBirthId)
                .WillCascadeOnDelete(false);

            HasRequired(c => c.City)
                .WithMany(g => g.ClientWaitingWhomBehzistiDocumentInThisCity)
                .HasForeignKey(c=>c.CityId)
                .WillCascadeOnDelete(false);


            HasRequired(c => c.HouseCity)
                .WithMany(g => g.ClientWaitingWhoTheirHouseIsInThisCity)
                .HasForeignKey(c=>c.HouseCityId)
                .WillCascadeOnDelete(false);



            HasRequired(c => c.ClientType)
                .WithMany(g => g.ClientWaitingLists)
                .HasForeignKey(c=>c.ClientTypeId)
                .WillCascadeOnDelete(false);



            HasRequired(c => c.CurrentHouseType)
                .WithMany(g => g.ClientWaitingLists)
                .HasForeignKey(c=>c.CurrentHouseTypeId)
                .WillCascadeOnDelete(false);

        }
    }
}