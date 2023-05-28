using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class UserFormViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "نام کاربر ضروری می باشد")]
        [Display(Name = "نام کاربر")]
        [StringLength(50, ErrorMessage = "نام کاربر باید بین 5 تا 50 کارکتر باشد", MinimumLength = 5)]
        [Remote("IsUserNameExist", "User", AdditionalFields = "UserId", ErrorMessage = "این نام کاربر قبلا استفاده شده است")]
        [RegularExpression("[a-z]+", ErrorMessage = "نام کاربر فقط می تواند شامل کاراکترهای انگلیسی باشد")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رمز عبور ضروری می باشد")]
        [StringLength(255, ErrorMessage = "رمز عبور باید بین {2} تا {1} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Required(ErrorMessage = "تاییدیه رمز عبور ضروری می باشد")]
        [StringLength(255, ErrorMessage = "رمز عبور باید بین {2} تا {1} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "رمز عبور و تاییدیه رمز عبور با هم مساوی نیستند")]
        [Display(Name = "تایید رمز عبور")]
        public string ConfirmPassword { get; set; }

        public string PasswordHash { get; set; }


        // ---------- Person Data ---------- 
        public UserInfo UserInfo { get; set; }

        public IEnumerable<UserRoleDto> UserRoles { get; set; }

        [Required(ErrorMessage = "انتخاب گروه کاربر ضروری می باشد")]
        [Display(Name = "گروه کاربر")]
        public List<string> UserRoleNames { get; set; }

        public bool CanManageClient { get; set; }


        public IEnumerable<ProvinceDto> Provinces { get; set; }

        public IEnumerable<CountyDto> Counties { get; set; }

        public IEnumerable<AssistanceType> AssistanceTypes { get; set; }

        public IEnumerable<CoOrganizationType> CoOrganizationTypes { get; set; }


        public int? TempImageId { get; set; }

        public bool IsSignUploaded { get; set; }
    }
}