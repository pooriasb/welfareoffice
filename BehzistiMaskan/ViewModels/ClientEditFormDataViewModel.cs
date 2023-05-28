using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.ViewModels
{
    public class ClientEditFormDataViewModel
    {
        public ClientDto Client { get; set; }

        public ClientForm ClientForm { get; set; }

        public Form Form { get; set; }

        public IEnumerable<FieldTemplate> FieldTemplates { get; set; }

        public string JsonFormFieldData { get; set; }


    }
}