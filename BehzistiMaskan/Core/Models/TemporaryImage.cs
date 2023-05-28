using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.FormBuilder;

namespace BehzistiMaskan.Core.Models
{
    public class TemporaryImage
    {
        public int Id { get; set; }

        public string MasterFileName { get; set; }

        public string TemporaryFileName { get; set; }

        public int? ClientId { get; set; }
        public Client Client { get; set; }

        public int? FieldId { get; set; }
        public Field Field { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpireAt { get; set; }

    }
}