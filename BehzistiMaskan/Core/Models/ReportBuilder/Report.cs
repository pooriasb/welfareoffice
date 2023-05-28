using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models.Geographic;

namespace BehzistiMaskan.Core.Models.ReportBuilder
{
    public class Report
    {
        public int Id { get; set; }

        [Display(Name = "عنوان گزارش")]
        [Required(ErrorMessage = "انتخاب عنوان برای گزارش ضروری می باشد")]
        [MinLength(4, ErrorMessage = "عنوان حداقل باید شامل {1} کاراکتر باشد")]
        public string ReportName { get; set; }

        public string CreatorName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdateAt { get; set; }


        public bool NationalCode { get; set; }

        public bool ClientIsDisabledShow { get; set; }

        public bool ClientName { get; set; }
        public bool ClientFamily { get; set; }
        public bool ClientFatherName { get; set; }
        public bool ClientMotherName { get; set; }
        public bool ClientBirthDate { get; set; }
        public bool ClientGender { get; set; }
        public bool ClientMarriageStatus { get; set; }
        public bool ClientChildrenCount { get; set; }
        public bool ClientCertificateNo { get; set; }
        public bool ClientCertificateMosalsal { get; set; }
        public bool ClientCertificateDescription { get; set; }
        public bool ClientProvinceOfBirth { get; set; }
        public bool ClientCountyOfBirth { get; set; }
        public bool ClientDistrictOfBirth { get; set; }
        public bool ClientCityOfBirth { get; set; }

        public bool ClientBehzistiDocumentCounty { get; set; }
        public bool ClientBehzistiDocumentDistrict { get; set; }
        public bool ClientBehzistiDocumentCity { get; set; }


        public bool ClientAssistance { get; set; }
        public bool ClientType { get; set; }
        public bool ClientDescription { get; set; }
        public bool BehzistiCode { get; set; }
        public bool GlobalBehzistiUiCode { get; set; }
        public bool NumberOfDisabledInFamily { get; set; }
        public bool IsHouseHold { get; set; }


        public bool CurrentHousing_CurrentHouseTypeId { get; set; }
        public bool CurrentHousing_DepositAmount { get; set; }
        public bool CurrentHousing_MonthlyRentalRate { get; set; }
        public bool CurrentHousing_Address { get; set; }
        public bool CurrentHousing_PostalCode { get; set; }


        public bool ContactInfo_HomeTel { get; set; }
        public bool ContactInfo_Mobile { get; set; }
        public bool ContactInfo_EmergencyTel { get; set; }


        public bool BankInfo_BankTypeId { get; set; }
        public bool BankInfo_AccountNumber { get; set; }
        public bool BankInfo_CardNumber { get; set; }

        public bool PhysicalProgress { get; set; }

        public ICollection<ReportCounty> ReportCounties { get; set; }
        public ICollection<ReportForm> ReportForms { get; set; }

        /// <summary>
        /// کد استانی که گزارش مربوط به آن می باشد
        /// </summary>
        public int? ProvinceId { get; set; }
        public Province Province { get; set; }

        /// <summary>
        /// کد شهرستانی که گزارش مربوط به آن می باشد
        /// </summary>
        public int? CountyId { get; set; }
        public County County { get; set; }
    }
}