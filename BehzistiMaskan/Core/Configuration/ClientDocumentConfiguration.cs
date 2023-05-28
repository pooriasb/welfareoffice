using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientDocumentConfiguration :EntityTypeConfiguration<ClientDocument>
    {
        public ClientDocumentConfiguration()
        {
            //Table Name
            ToTable("ClientDocument");

            //Key
            HasKey(c => c.Id);

            //Property
            Property(c => c.DocURI)
                .IsRequired();

            //Relation
            HasRequired(c=>c.DocumentType)
                .WithMany(d=>d.ClientDocuments)
                .HasForeignKey(c=>c.DocumentTypeId)
                .WillCascadeOnDelete(false);

            /*
             relation between --ClientDocument-- and --Client-- is declared on ClientConfiguration
             */
        }
    }
}