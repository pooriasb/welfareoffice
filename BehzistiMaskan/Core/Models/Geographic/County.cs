using System;
using System.Collections.Generic;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.ReportBuilder;

namespace BehzistiMaskan.Core.Models.Geographic
{
    public class County
    {
        public County()
        {
            Districts = new HashSet<District>();
            FormAccessLevels = new HashSet<FormAccessLevel>();
            UserInfos = new HashSet<UserInfo>();
        }
        public int Id { get; set; }

        public Guid UniqueId { get; set; }

        public string Name { get; set; }

        public int ProvinceId { get; set; }
        public Province Province { get; set; }

        public ICollection<District> Districts { get; set; }
        public ICollection<FormAccessLevel> FormAccessLevels { get; set; }

        public ICollection<UserInfo> UserInfos { get; set; }
        public ICollection<ReportCounty> ReportCounties { get; set; }

        public ICollection<Report> ReportsOfThisCounty { get; set; }
        public ICollection<RequiredMessage> RequiredMessages { get; set; }  
        public ICollection<RequiredMessage2> RequiredMessages2 { get; set; }


        public ICollection<Benefactor> BenefactorsOfThisCounty { get; set; }
        public RequiredMessage RequiredMessage { get; set; }
    }
}