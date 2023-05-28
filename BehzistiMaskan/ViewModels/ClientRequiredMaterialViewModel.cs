using System.Collections.Generic;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class ClientRequiredMaterialViewModel
    {
        public int ClientId { get; set; }
        public ClientDto Client { get; set; }

        public IEnumerable<MaterialType> AllMaterialTypes { get; set; }

        public IEnumerable<ClientRequiredMaterial> ClientRequiredMaterials { get; set; }
        
        public Status Status { get; set; }

        public ClientRequiredMaterial ClientRequiredMaterial { get; set; }
    }
}