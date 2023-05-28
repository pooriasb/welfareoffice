using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehzistiMaskan.Core.Models
{
    /// <summary>
    /// کلاس برای ذخیره سازی اطلاعات معرفی نامه های صادر شده برای دریافت معافیت انشعابات
    /// </summary>
    public class ClientRequestGetLetter
    {
        public int Id { get; set; }

        public int ClientRequestId { get; set; }
        public ClientRequest ClientRequest { get; set; }

        /// <summary>
        /// تاریخ صدور معرفی نامه
        /// </summary>
        public DateTime LetterDate { get; set; }

        /// <summary>
        /// شماره نامه معرفی
        /// </summary>
        public string LetterNumber { get; set; }

        
        public int LetterPhotoId { get; set; }
        public ClientDocument LetterPhoto { get; set; }
    }
}