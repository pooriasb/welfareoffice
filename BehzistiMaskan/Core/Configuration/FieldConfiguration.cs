using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Configuration
{
    public class FieldConfiguration : EntityTypeConfiguration<Field>
    {
        public FieldConfiguration()
        {
            //Table Name
            ToTable("Field");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(f => f.Title)
                .HasMaxLength(300)
                .IsRequired();

            Property(f => f.CreatedAt)
                .IsRequired();

            Property(f => f.IsRequired)
                .IsRequired();

            //relation between Field and Form are declared in FormConfiguration.cs

            HasMany(f => f.FieldMetas)
                .WithRequired(fm => fm.Field)
                .HasForeignKey(fm => fm.FieldId)
                .WillCascadeOnDelete(true);

            HasRequired(f=>f.FieldTemplate)
                .WithMany(f=>f.Fields)
                .HasForeignKey(f=>f.FieldTemplateName)
                .WillCascadeOnDelete(false);
        }
    }
}