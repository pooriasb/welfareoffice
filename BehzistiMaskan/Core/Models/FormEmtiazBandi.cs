using System;

namespace BehzistiMaskan.Core.Models
{
    public class FormEmtiazBandi
    {
        public int Id { get; set; }

        public Client Client { get; set; }

        //امتیاز
        public int Rate { get; set; }

        //مبلغ
        public long Amount { get; set; }

        // در صورتی که از فرم امتیاز بندی آفلاین استفاده کردیم این گزینه بایستی فعال شود و فایل اسکن فرم آپلود شود
        public bool IsApprovedOffline { get; set; }

        // قرار شد در صورت آفلاین یا آنلاین بودن اطلاعات فرم امتیاز بندی آپلود شود
        // فرم امتیاز بندی آفلاین دارای تمام پنج امضا بوده و اسکن شده است
        // اما فرم امتیاز بندی آنلاین به این معنی است که مددکار فرم را امضا و اسکن میکند و برای امضای بقیه افراد آن را در سامانه بارگذاری می نماید
        public ClientDocument OfflineFormEmtiazBandi { get; set; }

        // هر فرم بایستی توسط 5 نفر امضا شود

        // وقتی فرم آنلاین آپلود شود به منزل تایید توسط کارشناس شهرستان می باشد
        public bool IsApproveByKarshenasShahrestan { get; set; }
        public DateTime? DateOfKarshenasShahrestanApprove { get; set; }

        public bool IsApproveByModirShahrestan { get; set; }
        public DateTime? DateOfModirShahrestanApprove { get; set; }

        public bool IsApproveByRelatedAssistance { get; set; }
        public DateTime? DateOfRelatedAssistanceApprove { get; set; }

        public bool IsApproveByMoavenMosharekatAssistance { get; set; }
        public DateTime? DateOfMoavenMosharekatAssistanceApprove { get; set; }

        public bool IsApproveByModirKol { get; set; }
        public DateTime? DateOfModirKolApprove { get; set; }
    }
}