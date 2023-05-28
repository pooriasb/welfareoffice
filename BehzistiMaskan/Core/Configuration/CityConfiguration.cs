using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Configuration
{
    public class CityConfiguration : EntityTypeConfiguration<City>
    {
        public CityConfiguration()
        {
            //Table Name
            ToTable("City");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(c => c.Dehestan)
                .HasMaxLength(150);

            Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            Property(c => c.IsVillage)
                .IsRequired();

            //relation between --City-- and --District-- is declared in DistrictConfiguration
            //relation between --City-- and --Person-- is declared in PersonConfiguration
        }
    }
}