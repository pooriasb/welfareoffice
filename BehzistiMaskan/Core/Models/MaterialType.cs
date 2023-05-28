using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class MaterialType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PersianName { get; set; }

        public ICollection<ClientRequiredMaterial> RequiredMaterialsWithThisType { get; set; }

    }
}