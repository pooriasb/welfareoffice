using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Dtos
{
    public class CityDto
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        [Required]
        public bool IsVillage { get; set; }
        public string Dehestan { get; set; }

    }
}