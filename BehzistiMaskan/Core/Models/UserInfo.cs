using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;

namespace BehzistiMaskan.Core.Models
{
    public class UserInfo
    {
        public string Id { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "ورود نام ضروری می باشد")]
        public string Name { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "ورود نام خانوادگی ضروری می باشد")]
        public string Family { get; set; }
        [Display(Name = "تلفن همراه")]
        [StringLength(maximumLength: 11, MinimumLength = 11, ErrorMessage = "شماره موبایل به درستی وارد نشده است")]
        public string Mobile { get; set; }


        [Display(Name = "کاربر کدام استان می باشد")]
        public int? ProvinceId { get; set; }
        public Province Province { get; set; }

        [Display(Name = "کاربر کدام شهرستان می باشد؟")]
        public int? CountyId { get; set; }
        public County County { get; set; }

        [Display(Name = "کارمند کدام معاونت می باشد؟")]
        public int? AssistanceTypeId { get; set; }
        public AssistanceType AssistanceType { get; set; }

        public ApplicationUser User { get; set; }

        [Display(Name = "نام سازمان همکار")]
        public int? CoOrganizationTypeId { get; set; }
        public CoOrganizationType CoOrganizationType { get; set; }


        public string SignFullAddress { get; set; }

        public bool IsSignUploaded { get; set; }

        public ICollection<DownloadRequest> DownloadRequests { get; set; }
    }
}