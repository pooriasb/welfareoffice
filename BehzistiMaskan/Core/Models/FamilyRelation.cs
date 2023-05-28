using System;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class FamilyRelation
    {

        public int Id { get; set; }

        public int PersonMajorId { get; set; }
        public Person PersonMajor { get; set; }

        [Required(ErrorMessage = "انتخاب فرد ضروری می باشد")]
        public int PersonMinorId { get; set; }
        public Person PersonMinor { get; set; }

        [Display(Name = "نسبت با مددجو")]
        [Required(ErrorMessage = "انتخاب رابطه فامیلی فرد با مددجو ضروری می باشد")]
        public int FamilyRelationTypeId { get; set; }
        public FamilyRelationType FamilyRelationType { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

    }
}