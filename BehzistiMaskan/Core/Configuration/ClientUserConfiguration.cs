using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class ClientUserConfiguration : EntityTypeConfiguration<ClientUser>
    {

        public ClientUserConfiguration()
        {
            
            // Table

            ToTable("ClientUser");


            //Property

            HasKey(c => c.NationalCode);

            Property(c => c.NationalCode)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();

            Property(c => c.ActivationCode)
                .IsRequired()
                .HasMaxLength(6)
                .IsFixedLength();

        }
    }
}