using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.ReportBuilder;

namespace BehzistiMaskan.Core.Configuration
{
    public class ReportFormFieldConfiguration:EntityTypeConfiguration<ReportFormField>
    {
        public ReportFormFieldConfiguration()
        {
            HasRequired(ff=>ff.Field)
                .WithMany(f=>f.ReportFormFields)
                .HasForeignKey(ff=>ff.FieldId)
                .WillCascadeOnDelete(false);

            HasRequired(ff=>ff.ReportForm)
                .WithMany(rf=>rf.ReportFormFields)
                .HasForeignKey(ff=>ff.ReportFormId)
                .WillCascadeOnDelete(false);


        }
    }
}