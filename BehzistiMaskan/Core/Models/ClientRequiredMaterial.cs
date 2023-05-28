using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class ClientRequiredMaterial
    {
        public int Id { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        [Display(Name = "نوع مصالح مورد نیاز")]
        public int MaterialTypeId { get; set; }
        public MaterialType MaterialType { get; set; }

        [Display(Name = "مقدار یا تعداد")]
        [Required(ErrorMessage = "وارد کردن میزان مصالح ضروری می باشد")]
        [Range(1, long.MaxValue, ErrorMessage = "لطفا یک عدد وارد نمایید")]
        [RegularExpression("[1-9][0-9]*", ErrorMessage = "لطفا یک عدد وارد نمایید")]
        public long Count { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "وارد کردن توضیحات ضروری می باشد")]
        public string Description { get; set; }

        public DateTime Date { get; set; }
    }
}