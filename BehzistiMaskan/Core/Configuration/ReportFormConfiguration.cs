using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.ReportBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class ReportFormConfiguration : EntityTypeConfiguration<ReportForm>
    {
        public ReportFormConfiguration()
        {
            HasRequired(rf => rf.Form)
                .WithMany(f => f.ReportForms)
                .HasForeignKey(rf => rf.FormId)
                .WillCascadeOnDelete(false);

            HasRequired(rf => rf.Report)
                .WithMany(r => r.ReportForms)
                .HasForeignKey(rf => rf.ReportId)
                .WillCascadeOnDelete(false);
        }
    }
}