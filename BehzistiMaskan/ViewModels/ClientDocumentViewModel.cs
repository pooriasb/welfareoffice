
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.ViewModels
{
    public class ClientDocumentViewModel
    {

        public int ClientId { get; set; }
        public ClientDto Client { get; set; }
        public Status Status { get; set; }

        [Display(Name = "نوع مدرک")]
        [Required(ErrorMessage = "انتخاب {0} ضروری می باشد")]
        public int DocumentTypeId { get; set; }
        public IEnumerable<DocumentType> AllDocumentTypes { get; set; }

        [Required(ErrorMessage = "بارگذاری تصویر ضروری می باشد")]
        public int TempClientDocumentId { get; set; }

        public IEnumerable<ClientDocument> ClientDocuments { get; set; }

        /// <summary>
        /// آیا وضعیت مسکن فعلی را اجاره ای انتخاب کرده است؟
        /// </summary>
        public bool HasSelectRentalForCurrentHouse { get; set; }

        /// <summary>
        /// انتخاب کرده است که در منزل خیر سکونت دارد؟
        /// </summary>
        public bool HasSelectLiveInBenefactorHouse { get; set; }

    }
}