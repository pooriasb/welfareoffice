using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.Core.Configuration
{
    public class SystemSettingConfiguration:EntityTypeConfiguration<SystemSetting>
    {
        public SystemSettingConfiguration()
        {

            ToTable("SystemSettings");

            HasKey(s => s.Id);


            Property(s => s.Name)
                .HasMaxLength(300)
                .IsRequired();

            Property(s => s.Value)
                .HasMaxLength(300)
                .IsRequired();

            Property(s => s.ValueType)
                .HasMaxLength(25)
                .IsRequired();



        }
    }
}