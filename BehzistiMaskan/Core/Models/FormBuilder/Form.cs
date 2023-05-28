using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.ReportBuilder;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class Form
    {
        public Form()
        {
            FormMetas = new HashSet<FormMeta>();
            Fields = new HashSet<Field>();
            FormPhysicalProgresses = new HashSet<FormPhysicalProgress>();
            FormAccessLevels = new HashSet<FormAccessLevel>();
            ClientForms = new HashSet<ClientForm>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsEnabled { get; set; }

        public bool? IsDeleted { get; set; }

        public string FormStatus { get; set; }

        public long TotalQuota { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<FormMeta> FormMetas { get; set; }

        public ICollection<Field> Fields { get; set; }

        public ICollection<FormPhysicalProgress> FormPhysicalProgresses { get; set; }
        public ICollection<ReportForm> ReportForms { get; set; }

        public ICollection<FormAccessLevel> FormAccessLevels { get; set; }

        public ICollection<FormCoOrganizationRole> FormCoOrganizationRoles { get; set; }

        public ICollection<ClientForm> ClientForms { get; set; }


        public int ProvinceId { get; set; }

        public Province Province { get; set; }
    }
}