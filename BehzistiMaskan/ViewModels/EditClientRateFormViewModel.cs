using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.ViewModels
{
    public class EditClientRateFormViewModel
    {
        public ClientDto Client { get; set; }

        [Required(ErrorMessage = "آپلود تصویر فرم ضروری می باشد")]
        public int TempImageIdFormEmtiazBandi { get; set; }

        public FormEmtiazBandiDto FormEmtiazBandi { get; set; }
        [Required(ErrorMessage = "وارد کردن مبلغ ضروری می باشد")]
        [Display(Name = "مبلغ (ریال)")]
        public string Amount { get; set; }
        // برای مدیر کل و معاون مشارکت و معاون تخصصی این گزینه رو اضافه کردم
        public bool IsApprovedByThisUser { get; set; }

    }
}