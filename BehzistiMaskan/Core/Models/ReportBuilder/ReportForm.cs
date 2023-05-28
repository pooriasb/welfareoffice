using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Models.ReportBuilder
{
    public class ReportForm
    {
        public ReportForm()
        {
            ReportFormFields = new HashSet<ReportFormField>();
        }
        public int Id { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; }

        public ICollection<ReportFormField> ReportFormFields { get; set; }
    }
}