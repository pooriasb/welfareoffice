using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class JsonFormPostObject
    {
        public int Id { get; set; }
        public int Length { get; set; }

        public List<JsonFormField> Items { get; set; }

        public override string ToString()
        {
            return Json.Encode(this);
        }
    }
}