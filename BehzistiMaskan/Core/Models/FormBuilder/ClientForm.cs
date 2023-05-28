using System;
using System.Collections.Generic;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class ClientForm
    {
        public ClientForm()
        {
            ClientFormFields = new HashSet<ClientFormField>();
        }
        public int Id { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }

        public int FormId { get; set; }

        public Form Form { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<ClientFormField> ClientFormFields { get; set; }

    }
}