using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Configuration
{
    public class FormMetaConfiguration : EntityTypeConfiguration<FormMeta>
    {
        public FormMetaConfiguration()
        {
            //Table Name
            ToTable("FormMeta");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(f => f.Key)
                .HasMaxLength(200)
                .IsRequired();

            Property(f => f.Value)
                .HasMaxLength(2000)
                .IsRequired();


            //relation between Form and FormMeta are declared in FormConfiguration.cs
        }
    }
}