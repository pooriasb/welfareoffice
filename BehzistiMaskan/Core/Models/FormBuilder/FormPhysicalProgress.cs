using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class FormPhysicalProgress
    {
        public FormPhysicalProgress()
        {
            FormPhysicalProgressHelps = new HashSet<FormPhysicalProgressHelp>();
        }
        public int Id { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }
        public int PhysicalProgressId { get; set; }
        public PhysicalProgress PhysicalProgress { get; set; }

        public long BehzistiHelpAmount { get; set; }
        public ICollection<FormPhysicalProgressHelp> FormPhysicalProgressHelps { get; set; }
    }
}