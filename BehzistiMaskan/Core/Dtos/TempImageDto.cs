using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Dtos
{
    public class TempImageDto
    {
        public HttpPostedFileBase image { get; set; }

        public int clientId { get; set; }

        public int fieldId { get; set; }
    }
}