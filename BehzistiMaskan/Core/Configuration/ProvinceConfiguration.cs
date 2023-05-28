using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Configuration
{
    public class ProvinceConfiguration : EntityTypeConfiguration<Province>
    {
        public ProvinceConfiguration()
        {
            //Table Name
            ToTable("Province");

            //Key
            HasKey(p => p.Id);

            //Property
            Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            //Relations

            HasMany(p => p.Counties)
                .WithRequired(c => c.Province)
                .HasForeignKey(c => c.ProvinceId)
                .WillCascadeOnDelete(false);

            HasMany(p => p.ReportsOfThisProvince)
                .WithOptional(r => r.Province)
                .HasForeignKey(r => r.ProvinceId)
                .WillCascadeOnDelete(false);


        }
    }
}