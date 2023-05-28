using System;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models.Geographic;
namespace BehzistiMaskan.Core.Models
{
    public class RequiredMessage
    {
        public int Id { get; set; }

        [Display(Name="متن پیام")]
        [Required(ErrorMessage = "وارد کردن متن پیام ضروری می باشد")]
        [MinLength(10, ErrorMessage="متن پیام باید حداقل 10 کاراکتر باشد")]
        public string Message { get; set; }

        public DateTime UpdateDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public County County { get; set; }
        public int CountyId { get; set; }
    }
}