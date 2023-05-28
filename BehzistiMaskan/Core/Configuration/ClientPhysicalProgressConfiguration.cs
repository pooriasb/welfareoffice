using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientPhysicalProgressConfiguration : EntityTypeConfiguration<ClientPhysicalProgress>
    {
        public ClientPhysicalProgressConfiguration()
        {

            //Table Name
            ToTable("ClientPhysicalProgress");

            //Key
            HasKey(c => c.Id);

            //Property


            //Relations
            HasRequired(ph => ph.Client)
                .WithMany(c => c.ClientPhysicalProgresses)
                .HasForeignKey(ph=>ph.ClientId)
                .WillCascadeOnDelete(false);

            HasRequired(ph => ph.PhysicalProgress)
                .WithMany(p => p.ClientPhysicalProgresses)
                .HasForeignKey(ph => ph.PhysicalProgressId)
                .WillCascadeOnDelete(false);


            // relation to clientPhysicalProgressPhoto is declared in ClientPhysicalProgressPhotoConfiguration.cs
        }
    }
}