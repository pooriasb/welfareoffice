using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Utility;
using MD.PersianDateTime;

namespace BehzistiMaskan.ViewModels
{
    public class ClientPhysicalProgressViewModel
    {
        public ClientDto Client { get; set; }

        public IEnumerable<PhysicalProgress> AllPhysicalProgress { get; set; }

        public IEnumerable<ClientPhysicalProgress> ClientPhysicalProgresses { get; set; }

        [Display(Name = "تاریخ گرفتن عکس")]
        [DataType(DataType.Date, ErrorMessage = "تاریخ به درستی وارد نشده است")]
        public string PhotoTakenDate { get; set; }

        [Display(Name = "مرحله پیشرفت فیزیکی")]
        [Required(ErrorMessage = "انتخاب مرحله پیشرفت فیزیکی ضروری می باشد")]
        public int SelectedPhysicalProgressId { get; set; }

        [Required(ErrorMessage = "ثبت عکس پیشرفت فیزیکی ضروری می باشد")]
        public int? TemporaryImageId { get; set; }


        [Display(Name="توضیحات تصویر")]
        public string Description { get; set; }

        public IEnumerable<int> EnablePhysicalProgressIds { get; set; }
        public Status Status { get; set; }
    }
}