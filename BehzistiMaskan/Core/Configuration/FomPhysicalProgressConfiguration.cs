using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class FormPhysicalProgressConfiguration : EntityTypeConfiguration<FormPhysicalProgress>
    {
        public FormPhysicalProgressConfiguration()
        {
            //To Table
            ToTable("FormPhysicalProgress");

            //Key
            HasKey(p => p.Id);

            //Property

            //Relation
            HasRequired(fp => fp.PhysicalProgress)
                .WithMany(p => p.FormPhysicalProgresses)
                .HasForeignKey(fp => fp.PhysicalProgressId);


            //relation between form and form Physical Progress store in FormConfiguration
            //relation between form physical progress and form physical progress help stored in FormPhysicalProgressHelpConfiguration.cs

            HasMany(fp => fp.FormPhysicalProgressHelps)
                .WithRequired(h => h.FormPhysicalProgress)
                .HasForeignKey(h => h.FormPhysicalProgressId);
        }
    }
}