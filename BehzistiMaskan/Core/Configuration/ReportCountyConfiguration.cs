using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.ReportBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class ReportCountyConfiguration:EntityTypeConfiguration<ReportCounty>
    {
        public ReportCountyConfiguration()
        {
            
            HasRequired(rc=>rc.Report)
                .WithMany(r=>r.ReportCounties)
                .HasForeignKey(rc=>rc.ReportId)
                .WillCascadeOnDelete(false);

            HasRequired(rc=>rc.County)
                .WithMany(c=>c.ReportCounties)
                .HasForeignKey(rc=>rc.CountyId)
                .WillCascadeOnDelete(false);

        }
    }
}