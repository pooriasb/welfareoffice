using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BehzistiMaskan.Core.Models.Utility
{
    public class DocumentType
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public ICollection<ClientDocument> ClientDocuments { get; set; }

        public bool IsBasic { get; set; }

        [StringLength(255)]
        public string PersianName { get; set; }
    }
}