using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class JsonFormField
    {
        public int Id { get; set; }

        public string FieldTemplateName { get; set; }

        public string Title { get; set; }

        public int FormId { get; set; }

        public bool? IsRequired { get; set; }

        public string HelpText { get; set; }

        public bool IsHtmlHelp { get; set; }
    }
}