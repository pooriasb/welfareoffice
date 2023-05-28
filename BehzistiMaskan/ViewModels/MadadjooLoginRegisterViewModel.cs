using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Dtos;

namespace BehzistiMaskan.ViewModels
{
    public class MadadjooLoginRegisterViewModel
    {
        [Required(ErrorMessage = "وارد کردن کد ملی ضروری می باشد")]
        [StringLength(10, ErrorMessage = "کد ملی به صورت صحیح وارد نشده است", MinimumLength = 10)]
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Required(ErrorMessage = "وارد کردن کد رهگیری ضروری می باشد")]
        [Display(Name = "کد رهگیری")]
        public string LoginActivationCode { get; set; }

        [Required(ErrorMessage = "وارد کردن کد امنیتی ضروری می باشد")]
        [Display(Name = "کد امنیتی")]
        public string SecurityAnswer { get; set; }

        public SecurityQuestionDto SecurityQuestionDto { get; set; }
    }
}