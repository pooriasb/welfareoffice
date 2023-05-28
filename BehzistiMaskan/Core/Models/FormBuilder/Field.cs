using System;
using System.Collections.Generic;
using System.Linq;
using BehzistiMaskan.Core.Models.ReportBuilder;
using BehzistiMaskan.Models;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class Field
    {
        public Field()
        {
            ClientFormFields = new HashSet<ClientFormField>();
        }
        public int Id { get; set; }

        public string Title { get; set; }

        public int FormId { get; set; }

        public Form Form { get; set; }

        public string FieldTemplateName { get; set; }
        public FieldTemplate FieldTemplate { get; set; }

        public ICollection<FieldMeta> FieldMetas { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsRequired { get; set; }

        public string HelpText { get; set; }

        public bool? IsHtmlHelp { get; set; }

        public ICollection<ClientFormField> ClientFormFields { get; set; }

        public ICollection<TemporaryImage> TemporaryImages { get; set; }
        public ICollection<ReportFormField> ReportFormFields { get; set; }

    }
}