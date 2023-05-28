using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Models.ReportBuilder
{
    public class ReportCounty
    {
        public int Id { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; }


        public int CountyId { get; set; }

        public County County { get; set; }
    }
}