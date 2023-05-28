using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class FormMeta
    {
        public int Id { get; set; }

        public int FormId { get; set; }

        public Form Form { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}