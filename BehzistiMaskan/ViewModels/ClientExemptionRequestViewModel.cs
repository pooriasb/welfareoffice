using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Utility;
using MD.PersianDateTime;

namespace BehzistiMaskan.ViewModels
{
    public class ClientExemptionRequestViewModel
    {

        public int ClientId { get; set; }
        public ClientDto Client { get; set; }
        public Status Status { get; set; }

        public IEnumerable<ClientExemptionDetailViewModel> ClientExemptionDetails { get; set; }


        // مربوط به ثبت معرفی نامه معافیت
        [Display(Name = "نوع معافیت")]
        [Required(ErrorMessage = "انتخاب نوع معافیت ضروری می باشد")]
        public int GetLetterRequestTypeId { get; set; }

        [Display(Name = "تاریخ صدور معرفی نامه")]
        [Required(ErrorMessage = "تاریخ صدور معرفی نامه ضروری می باشد")]
        [DataType(DataType.Date)]
        public string GetFormLetterDate { get; set; }

        [Display(Name = "شماره معرفی نامه")]
        [Required(ErrorMessage = "شماره معرفی نامه ضروری می باشد")]
        public string GetFormLetterNumber { get; set; }

        [Required(ErrorMessage = "عکس معرفی نامه ضروری می باشد")]
        public int? GetFormTemporaryImageId { get; set; }

        // مربوط به ثبت بهره مندی معافیت انشعابات

        [Display(Name = "نوع معافیت")]
        [Required(ErrorMessage = "انتخاب نوع معافیت ضروری می باشد")]
        public int BenefitFormRequestTypeId { get; set; }

        [Display(Name = "تاریخ بهره مندی")]
        [Required(ErrorMessage = "تاریخ صدور معرفی نامه ضروری می باشد")]
        [DataType(DataType.Date)]
        public string BenefitFormLetterDate { get; set; }

        [Display(Name = "میزان معافیت (مبلغ)")]
        [Required(ErrorMessage = "میزان بهره مندی ضروری می باشد")]
        public string BenefitAmount { get; set; }

        [Required(ErrorMessage = "عکس معرفی نامه ضروری می باشد")]
        public int? BenefitFormTemporaryImageId { get; set; }

    }
}