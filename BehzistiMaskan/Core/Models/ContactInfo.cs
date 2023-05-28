using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class ContactInfo
    {
        public int Id { get; set; }

        [Display(Name = "آدرس محل کار")]
        public string WorkAddress { get; set; }

        [Display(Name = "تلفن منزل")]
        public string HomeTel { get; set; }

        [Display(Name = "تلفن محل کار")]
        public string WorkTel { get; set; }

        [Display(Name = "موبایل (تلفن همراه)")]
        [Required(ErrorMessage = "وارد کردن موبایل ضروری می باشد")]
        [MaxLength(11, ErrorMessage = "شماره ضروری به صورت صحیح وارد نشده است")]
        public string Mobile { get; set; }

        [Display(Name = "موبایل جهت تماس های ضروری")]
        [Required(ErrorMessage = "وارد کردن تلفن اضطراری ضروری می باشد")]
        [MaxLength(11, ErrorMessage = "شماره ضروری به صورت صحیح وارد نشده است")]
        public string EmergencyTel { get; set; }

        public Client Client { get; set; }
    }
}