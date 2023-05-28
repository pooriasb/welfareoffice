using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Configuration
{
    public class DistrictConfiguration : EntityTypeConfiguration<District>
    {
        public DistrictConfiguration()
        {
            //Table Name
            ToTable("District");

            //Key
            HasKey(d => d.Id);

            //Property
            Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(150);

            //Relations
            HasMany(d => d.Cities)
                .WithRequired(c => c.District)
                .HasForeignKey(c => c.DistrictId)
                .WillCascadeOnDelete(false);

            //relation between --County-- and --District-- is declared in CountyConfiguration
        }
    }
}