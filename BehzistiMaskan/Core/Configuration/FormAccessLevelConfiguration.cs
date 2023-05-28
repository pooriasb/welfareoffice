using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class FormAccessLevelConfiguration : EntityTypeConfiguration<FormAccessLevel>
    {
        public FormAccessLevelConfiguration()
        {
            //Table Name
            ToTable("FormAccessLevel");

            //Key
            HasKey(c => c.Id);

            //Property

            //Relations
            HasRequired(f => f.Form)
                .WithMany(f => f.FormAccessLevels)
                .HasForeignKey(f => f.FormId)
                .WillCascadeOnDelete(false);

            HasRequired(f => f.County)
                .WithMany(c => c.FormAccessLevels)
                .HasForeignKey(f => f.CountyId)
                .WillCascadeOnDelete(false);
        }
    }
}