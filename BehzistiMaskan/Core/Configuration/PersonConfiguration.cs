using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class PersonConfiguration : EntityTypeConfiguration<Person>
    {
        public PersonConfiguration()
        {
            //Table Name
            ToTable("Person");

            //Key
            HasKey(p => p.Id);


            //Property
            Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(250);

            Property(p => p.Family)
                .IsRequired()
                .HasMaxLength(250);

            Property(p => p.FatherName)
                .HasMaxLength(250);

            Property(p => p.MotherName)
                .HasMaxLength(250);

            Property(p => p.NationalCode)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnAnnotation(
                    "IndexNationalCode",
                    new IndexAnnotation(new IndexAttribute("IX_NationalCode") { IsUnique = true }));

            Property(p => p.BirthCertificateNo)
                .HasMaxLength(10);

            Property(p => p.BirthCertificateMosalsal)
                .HasMaxLength(15);

            Property(p => p.BirthCertificateDescription)
                .HasMaxLength(2000);

            Property(p => p.BirthCertificateNo)
                .HasMaxLength(10);

            //Relations
            HasOptional(p => p.CityOfBirth)
                .WithMany(c => c.PersonsWhoBirthIsInThisCity)
                .HasForeignKey(p => p.CityOfBirthId)
                .WillCascadeOnDelete(false);

            HasMany(p => p.FamilyRelations)
                .WithRequired(f => f.PersonMajor)
                .HasForeignKey(f => f.PersonMajorId)
                .WillCascadeOnDelete(false);

            HasMany(p => p.FamilyRelations)
                .WithRequired(f => f.PersonMinor)
                .HasForeignKey(f => f.PersonMinorId)
                .WillCascadeOnDelete(false);

            HasOptional(p => p.MarriageType)
                .WithMany(m => m.Persons)
                .HasForeignKey(p => p.MarriageTypeId)
                .WillCascadeOnDelete(false);

            HasOptional(p => p.GenderType)
                .WithMany(g => g.Persons)
                .HasForeignKey(p => p.GenderTypeId)
                .WillCascadeOnDelete(false);


            /*
             *  relation between --Person-- AND --Client-- is declared in --ClientConfiguration--
             */
        }
    }
}