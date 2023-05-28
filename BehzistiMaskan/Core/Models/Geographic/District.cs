using System;
using System.Collections.Generic;

namespace BehzistiMaskan.Core.Models.Geographic
{
    public class District
    {

        public District()
        {
            Cities = new HashSet<City>();
        }
        public int Id { get; set; }

        public Guid UniqueId { get; set; }
        public string Name { get; set; }

        public int CountyId { get; set; }
        public County County { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}