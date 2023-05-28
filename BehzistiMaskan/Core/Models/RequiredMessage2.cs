using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Models
{
    public class RequiredMessage2
    {

        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreateDateTime { get; set; }
        public County County { get; set; }
        public int CountyId { get; set; }
    }
}