using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Utility;
using Microsoft.Ajax.Utilities;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class FieldTemplate
    {
        [Key]
        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ModelEnums.FieldTemplateE Type { get; set; }

        public string AdminTemplateFileName { get; set; }
        public string UserTemplateFileName { get; set; }

        [NotMapped]
        private string adminHtmlTemplate;
        [NotMapped]
        public string AdminHtmlTemplate
        {
            get
            {
                if (this.adminHtmlTemplate.IsNullOrWhiteSpace() || this.adminHtmlTemplate.IsNullOrWhiteSpace())
                {
                    var templatePath = $"{UsefulMethods.GetFieldTemplatePath()}Admin\\{this.AdminTemplateFileName}";
                    if (File.Exists(templatePath))
                    {
                        this.adminHtmlTemplate = File.ReadAllText(templatePath);
                    }
                }
                return this.adminHtmlTemplate;
            }
            set { this.adminHtmlTemplate = value; }
        }

        [NotMapped]
        private string userHtmlTemplate;
        [NotMapped]
        public string UserHtmlTemplate
        {
            get
            {
                if (this.userHtmlTemplate.IsNullOrWhiteSpace() || this.userHtmlTemplate.IsNullOrWhiteSpace())
                {
                    var templatePath = $"{UsefulMethods.GetFieldTemplatePath()}User\\{this.UserTemplateFileName}";
                    if (File.Exists(templatePath))
                    {
                        this.userHtmlTemplate = File.ReadAllText(templatePath);
                    }
                }
                return this.userHtmlTemplate;
            }
            set { this.userHtmlTemplate = value; }
        }

        public ICollection<Field> Fields { get; set; }
    }
}