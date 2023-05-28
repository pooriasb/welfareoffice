using System;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class ClientFormField
    {
        public int Id { get; set; }

        public int ClientFormId { get; set; }

        public ClientForm ClientForm { get; set; }

        public int FieldId { get; set; }
        public Field Field { get; set; }

        public string Value { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}