using System;
using System.ComponentModel.DataAnnotations;

namespace BehzistiMaskan.Core.Dtos
{
    public class CountyDto
    {
        public int Id { get; set; }
        public Guid UniqueId { get; set; }
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
    }
}