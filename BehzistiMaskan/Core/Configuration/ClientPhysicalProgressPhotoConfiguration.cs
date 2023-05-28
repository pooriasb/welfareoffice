using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientPhysicalProgressPhotoConfiguration : EntityTypeConfiguration<ClientPhysicalProgressPhoto>
    {
        public ClientPhysicalProgressPhotoConfiguration()
        {

            //Table Name
            ToTable("ClientPhysicalProgressPhoto");

            //Key
            HasKey(c => c.Id);

            //Property


            //Relations
            HasRequired(ph => ph.ClientDocument)
                .WithOptional(cd => cd.ClientPhysicalProgressPhoto);

            HasRequired(ph => ph.ClientPhysicalProgress)
                .WithMany(cph => cph.ClientPhysicalProgressPhotos)
                .HasForeignKey(ph => ph.ClientPhysicalProgressId)
                .WillCascadeOnDelete(false);

        }
    }
}