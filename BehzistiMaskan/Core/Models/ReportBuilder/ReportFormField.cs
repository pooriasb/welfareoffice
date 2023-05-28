using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Models.ReportBuilder
{
    public class ReportFormField
    {
        public int Id { get; set; }

        public int ReportFormId { get; set; }
        public ReportForm ReportForm { get; set; }

        public int FieldId { get; set; }
        public Field Field { get; set; }
    }
}