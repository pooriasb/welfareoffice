using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Configuration
{
    public class PhysicalProgressConfiguration : EntityTypeConfiguration<PhysicalProgress>
    {
        public PhysicalProgressConfiguration()
        {
            //Table Name
            ToTable("PhysicalProgress");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            Property(p => p.Order)
                .IsRequired();

            //relation between physical configuration and form are declared in form

        }
    }
}