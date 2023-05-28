using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class FormPhysicalProgressHelpConfiguration : EntityTypeConfiguration<FormPhysicalProgressHelp>
    {
        public FormPhysicalProgressHelpConfiguration()
        {
            //To Table
            ToTable("FormPhysicalProgressHelp");

            //Key
            HasKey(p => p.Id);

            //Property

            //Relation
            HasRequired(h => h.FormPhysicalProgress)
                .WithMany(fp => fp.FormPhysicalProgressHelps)
                .HasForeignKey(h => h.FormPhysicalProgressId);

            HasOptional(h => h.CoOrganizationType)
                .WithMany(c => c.FormPhysicalProgressHelps)
                .HasForeignKey(h => h.CoOrganizationTypeId);
        }
    }
}