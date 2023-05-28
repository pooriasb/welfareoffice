using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Configuration
{
    public class FormEmtiazBandiConfiguration : EntityTypeConfiguration<FormEmtiazBandi>
    {
        public FormEmtiazBandiConfiguration()
        {
            ToTable("FormEmtiazBandi");

            Property(f=>f.Amount)
                .IsRequired();
            
            Property(f=>f.Rate)
                .IsRequired();

            HasRequired(f => f.Client)
                .WithOptional(c => c.FormEmtiazBandi);

            HasOptional(f => f.OfflineFormEmtiazBandi)
                .WithOptionalDependent(cd => cd.FormEmtiazBandi);
        }
    }
}