using System.Data.Entity.ModelConfiguration;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Configuration
{
    public class FormConfiguration : EntityTypeConfiguration<Form>
    {
        public FormConfiguration()
        {
            //Table Name
            ToTable("Form");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(f => f.Name)
                .HasMaxLength(300)
                .IsRequired();

            Property(f => f.Description)
                .HasMaxLength(2000);

            Property(f => f.CreatedAt)
                .IsRequired();
            
            
            //relation
            //HasMany(f => f.FormPhysicalProgresses)
            //    .WithMany(p => p.Forms)
            //    .Map(m =>
            //    {
            //        m.ToTable("FormPhysicalProgress");
            //        m.MapLeftKey("FormId");
            //        m.MapRightKey("PhysicalProgressId");
            //    });

            HasMany(f => f.FormPhysicalProgresses)
                .WithRequired(p => p.Form)
                .HasForeignKey(p => p.FormId);
                
            HasMany(f => f.Fields)
                .WithRequired(fi => fi.Form)
                .HasForeignKey(fi => fi.FormId)
                .WillCascadeOnDelete(false);

            HasMany(f => f.FormMetas)
                .WithRequired(fm => fm.Form)
                .HasForeignKey(fm => fm.FormId)
                .WillCascadeOnDelete(false);

            HasRequired(f => f.Province)
                .WithMany(p => p.Forms)
                .HasForeignKey(f => f.ProvinceId)
                .WillCascadeOnDelete(false);

        }
    }
}