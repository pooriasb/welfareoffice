using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class FormCoOrganizationRoleConfiguration : EntityTypeConfiguration<FormCoOrganizationRole>
    {
        public FormCoOrganizationRoleConfiguration()
        {
            //Table Name
            ToTable("FormCoOrganizationRole");

            //Key
            HasKey(c => c.Id);

            //Property

            //Relations
            HasRequired(f => f.Form)
                .WithMany(f => f.FormCoOrganizationRoles)
                .HasForeignKey(f => f.FormId)
                .WillCascadeOnDelete(false);

            HasRequired(f => f.CoOrganizationType)
                .WithMany(c => c.FormCoOrganizationRoles)
                .HasForeignKey(f => f.CoOrganizationTypeId)
                .WillCascadeOnDelete(false);

        }
    }
}