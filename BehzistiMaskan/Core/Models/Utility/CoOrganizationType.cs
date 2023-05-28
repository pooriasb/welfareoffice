using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Models.Utility
{
    public class CoOrganizationType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<FormPhysicalProgressHelp> FormPhysicalProgressHelps { get; set; }
        public ICollection<FormCoOrganizationRole> FormCoOrganizationRoles { get; set; }

        public ICollection<UserInfo> UserInfos { get; set; }
        public ICollection<CoOrganizationApproveList> CoOrganizationApproveLists { get; set; }

        public virtual RequestType RequestType { get; set; }
    }
}