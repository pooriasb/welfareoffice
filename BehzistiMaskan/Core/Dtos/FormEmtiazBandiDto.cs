using System.ComponentModel.DataAnnotations;
using BehzistiMaskan.Core.Models;
using MD.PersianDateTime;

namespace BehzistiMaskan.Core.Dtos
{
    public class FormEmtiazBandiDto
    {
        public int Id { get; set; }

        public Client Client { get; set; }

        //امتیاز
        [Display(Name = "امتیاز")]
        [Required(ErrorMessage = "وارد کردن امتیاز ضروری می باشد")]
        [Range(1,100, ErrorMessage = "{0} بایستی بین {1} تا {2} باشد.")]
        public int Rate { get; set; }

        //مبلغ
        [Required(ErrorMessage = "وارد کردن مبلغ ضروری می باشد")]
        [Display(Name = "مبلغ (ریال)")]
        public long Amount { get; set; }

        // در صورتی که از فرم امتیاز بندی آفلاین استفاده کردیم این گزینه بایستی فعال شود و فایل اسکن فرم آپلود شود
        [Display(Name = "نوع فرم")]
        public bool IsApprovedOffline { get; set; }

        // قرار شد در صورت آفلاین یا آنلاین بودن اطلاعات فرم امتیاز بندی آپلود شود
        // فرم امتیاز بندی آفلاین دارای تمام پنج امضا بوده و اسکن شده است
        // اما فرم امتیاز بندی آنلاین به این معنی است که مددکار فرم را امضا و اسکن میکند و برای امضای بقیه افراد آن را در سامانه بارگذاری می نماید
        public ClientDocument OfflineFormEmtiazBandi { get; set; }

        // هر فرم بایستی توسط 5 نفر امضا شود

        // وقتی فرم آنلاین آپلود شود به منزل تایید توسط کارشناس شهرستان می باشد
        public bool IsApproveByKarshenasShahrestan { get; set; }
        public PersianDateTime? DateOfKarshenasShahrestanApprove { get; set; }

        public bool IsApproveByModirShahrestan { get; set; }
        public PersianDateTime? DateOfModirShahrestanApprove { get; set; }

        public bool IsApproveByRelatedAssistance { get; set; }
        public PersianDateTime? DateOfRelatedAssistanceApprove { get; set; }

        public bool IsApproveByMoavenMosharekatAssistance { get; set; }
        public PersianDateTime? DateOfMoavenMosharekatAssistanceApprove { get; set; }

        public bool IsApproveByModirKol { get; set; }
        public PersianDateTime? DateOfModirKolApprove { get; set; }
    }
}