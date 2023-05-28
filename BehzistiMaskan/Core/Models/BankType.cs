using System.Collections.Generic;

namespace BehzistiMaskan.Core.Models
{
    public class BankType
    {
        public BankType()
        {
            FinanceInfos = new HashSet<BankInfo>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public ICollection<BankInfo> FinanceInfos { get; set; }
    }
}