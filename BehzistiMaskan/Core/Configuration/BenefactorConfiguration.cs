using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class BenefactorConfiguration : EntityTypeConfiguration<Benefactor>
    {
        public BenefactorConfiguration()
        {
            //Table Name
            ToTable("Benefactor");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(b => b.Name)
                .HasMaxLength(150)
                .IsRequired();
            Property(b => b.Family)
                           .HasMaxLength(150)
                           .IsRequired();
            Property(b => b.NationalCode)
                .HasMaxLength(10)
                .IsFixedLength()
                .IsRequired();

            Property(b => b.Mobile)
                .HasMaxLength(11)
                .IsFixedLength()
                .IsRequired();

            //Relations
            HasOptional(p => p.MarriageType)
                .WithMany(m => m.Benefactors)
                .HasForeignKey(p => p.MarriageTypeId)
                .WillCascadeOnDelete(false);

            HasOptional(p => p.GenderType)
                .WithMany(g => g.Benefactors)
                .HasForeignKey(p => p.GenderTypeId)
                .WillCascadeOnDelete(false);

            HasOptional(b => b.County)
                .WithMany(c => c.BenefactorsOfThisCounty)
                .HasForeignKey(b => b.CountyId);

            HasRequired(b => b.Province)
                .WithMany(p => p.BenefactorsOfThisProvince)
                .HasForeignKey(b => b.ProvinceId)
                .WillCascadeOnDelete(false);

            // Relation between --Benefactor-- and --BenefactorPayment-- is declared in BenefactorConfiguration.cs

        }
    }
}