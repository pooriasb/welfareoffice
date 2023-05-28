using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models.FormBuilder;
using MD.PersianDateTime;

namespace BehzistiMaskan.Core.Dtos
{
    public class FormDto
    {
        public int Id { get; set; }

        [Display(Name = "نام طرح")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "توضیحات طرح")]
        public string Description { get; set; }

        [Display(Name = "آیا طرح فعال است؟")]
        [Required]
        public bool IsEnabled { get; set; }

        public bool? IsDeleted { get; set; }

        public string FormStatus { get; set; }

        [Display(Name = "سهمیه کل")]
        [RegularExpression("[0-9]+", ErrorMessage = "مقدار وارد شده نادرست می باشد")]
        [Required(ErrorMessage = "وارد کردن فیلد سهمیه کل ضروری می باشد")]
        public long TotalQuota { get; set; }

        public string CreatedAt { get; set; }

        public string UpdatedAt { get; set; }

        public string AccessLevelStr { get; set; }

        public string CoOrganizationRoleStr { get; set; }

        public ICollection<Field> Fields { get; set; }
    }
}