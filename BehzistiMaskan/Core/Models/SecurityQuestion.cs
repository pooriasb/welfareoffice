using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class SecurityQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime Expire { get; set; }
    }
}