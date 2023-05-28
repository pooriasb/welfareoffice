using System.Collections.Generic;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.ReportBuilder;

namespace BehzistiMaskan.ViewModels
{
    public class GenerateReportViewModel
    {
        public Report Report { get; set; }

        public IEnumerable<Client> ReportResult { get; set; }

        public IEnumerable<PhysicalProgress> PhysicalProgresses { get; set; }
    }
}