using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    public class BankInfo
    {
        public int Id { get; set; }
        [Display(Name = "شماره حساب")]
        [Required(ErrorMessage = "وارد کردن شماره حساب ضروری می باشد")]
        [MinLength(4, ErrorMessage = "شماره حساب به درستی وارد نشده است")]
        public string AccountNumber { get; set; }

        [Display(Name = "شماره کارت")]
        [Required(ErrorMessage = "وارد کردن شماره کارت ضروری می باشد")]
        [StringLength(maximumLength:16, MinimumLength = 16, ErrorMessage = "شماره کارت به درستی وارد نشده است")]
        public string CardNumber { get; set; }

        [Display(Name = "نام بانک")]
        [Required(ErrorMessage = "انتخاب بانک ضروری می باشد")]
        public int BankTypeId { get; set; }
        public BankType BankType{ get; set; }

        public Client Client { get; set; }

        public int? AccountApproveImageId { get; set; }
        public ClientDocument AccountApproveImage { get; set; }

    }
}