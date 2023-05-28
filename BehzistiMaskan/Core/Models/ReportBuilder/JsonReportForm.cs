using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models.ReportBuilder
{
    public class JsonReportForm
    {
        public int FormId { get; set; }

        public IEnumerable<JsonReportFormField> JsonReportFormFields { get; set; }
    }
}