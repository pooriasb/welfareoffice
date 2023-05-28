using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientRequestGetLetterConfiguration : EntityTypeConfiguration<ClientRequestGetLetter>
    {
        public ClientRequestGetLetterConfiguration()
        {
            //Table Name

            // Key
            HasKey(gl => gl.Id);

            //Property
            Property(gl => gl.LetterNumber)
                .HasMaxLength(25)
                .IsRequired();


            //Relations
            HasRequired(gl => gl.ClientRequest)
                .WithMany(cr => cr.GetLetters)
                .HasForeignKey(gl => gl.ClientRequestId)
                .WillCascadeOnDelete(false);

            HasRequired(gl=>gl.LetterPhoto)
                .WithMany(cd=>cd.ClientRequestGetLetters)
                .HasForeignKey(gl=>gl.LetterPhotoId)
                .WillCascadeOnDelete(false);
        }
    }
}