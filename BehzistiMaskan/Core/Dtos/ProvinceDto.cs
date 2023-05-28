using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BehzistiMaskan.Core.Dtos
{
    public class ProvinceDto
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }

        [Required]
        public string Name { get; set; }

    }
}