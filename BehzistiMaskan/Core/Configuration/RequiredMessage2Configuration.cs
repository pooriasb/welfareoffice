using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class RequiredMessage2Configuration :EntityTypeConfiguration<RequiredMessage2>
    {
        public RequiredMessage2Configuration()
        {
            //table
            ToTable("RequiredMessage2");
            //key
            HasKey(x => x.Id);
            //relation
            HasRequired(re=>re.County)
                .WithMany(c=>c.RequiredMessages2)
                .HasForeignKey(re=>re.CountyId)
                .WillCascadeOnDelete(false);
        }
    }
}