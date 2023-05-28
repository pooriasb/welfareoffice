using System;
using System.Collections.Generic;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.ReportBuilder;

namespace BehzistiMaskan.Core.Models.Geographic
{
    public class Province
    {
        public Province()
        {
            Counties = new HashSet<County>();
            UserInfos = new HashSet<UserInfo>();
        }
        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public ICollection<County> Counties { get; set; }

        public ICollection<UserInfo> UserInfos{ get; set; }
        public ICollection<Form> Forms { get; set; }

        public ICollection<Report> ReportsOfThisProvince { get; set; }

        public ICollection<Benefactor> BenefactorsOfThisProvince { get; set; }

    }
}