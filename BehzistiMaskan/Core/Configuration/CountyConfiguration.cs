using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Configuration
{
    public class CountyConfiguration : EntityTypeConfiguration<County>
    {
        public CountyConfiguration()
        {

            //Table Name
            ToTable("County");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            //Relations
            HasMany(c => c.Districts)
                .WithRequired(d => d.County)
                .HasForeignKey(d => d.CountyId)
                .WillCascadeOnDelete(false);

            //relation between --County-- and --Province-- is declared in ProvinceConfiguration

            HasMany(p => p.ReportsOfThisCounty)
                .WithOptional(r => r.County)
                .HasForeignKey(r => r.CountyId)
                .WillCascadeOnDelete(false);

            HasOptional(c => c.RequiredMessage)
                .WithRequired(r => r.County)
                .WillCascadeOnDelete(false);
        }
    }
}