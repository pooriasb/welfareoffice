using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Configuration
{
    public class RequestTypeConfiguration : EntityTypeConfiguration<RequestType>
    {
        public RequestTypeConfiguration()
        {
            //Table Name
            ToTable("RequestType");

            //Key
            HasKey(c => c.Id);

            //Property

            Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(150);

            Property(r => r.PersianTitle)
                .IsRequired()
                .HasMaxLength(200);

            Property(r => r.IsExemption)
                .IsRequired();

            Property(r => r.PersianShortTitle)
                .IsRequired()
                .HasMaxLength(50);

            //Relations

            // برای نگه داشتن رابطه بین معافیت انشعابات آب و برق و گاز با سازمان های همکار

            HasOptional(rt => rt.CoOrganizationType)
                .WithOptionalDependent(org=>org.RequestType);
        }
    }
}