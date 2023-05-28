using System.Collections.Generic;

namespace BehzistiMaskan.ViewModels
{
    public class DashboardFormDataByPhysicalProgress
    {
        public string FormName { get; set; }

        public IEnumerable<FormSimplePhysicalProgress> FormDataByPhysicalProgresses { get; set; }

    }
}