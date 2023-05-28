using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models.FormBuilder
{
    public class ClientPhysicalProgressPhoto
    {
        public int Id { get; set; }

        public ClientDocument ClientDocument { get; set; }

        public int ClientPhysicalProgressId { get; set; }
        public ClientPhysicalProgress ClientPhysicalProgress { get; set; }

        public DateTime? PhotoTakenDate { get; set; }

        public long? GPSLatitude { get; set; }

        public long? GPSLongitude { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool? IsDeleted { get; set; }
    }
}