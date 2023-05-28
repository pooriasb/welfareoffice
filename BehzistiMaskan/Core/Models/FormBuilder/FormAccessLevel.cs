using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class FormAccessLevel
    {
        public int Id { get; set; }

        public int FormId { get; set; }
        public Form Form { get; set; }

        public int CountyId { get; set; }
        public County County { get; set; }

        public long Quota { get; set; }
    }
}