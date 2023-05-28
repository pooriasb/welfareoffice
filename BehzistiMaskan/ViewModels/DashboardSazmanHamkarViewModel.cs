using System.Collections.Generic;

namespace BehzistiMaskan.ViewModels
{
    public class DashboardSazmanHamkarViewModel
    {
        public int ClientCount { get; set; }

        public IEnumerable<DashboardFormSimpleData> DashboardFormSimpleDatas { get; set; }

        public IEnumerable<DashboardFormSimpleData> DashboardFormByMoney { get; set; }
    }
}