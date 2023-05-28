using System;
using System.Collections.Generic;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class ClientWaitingApplicant
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Family { get; set; }

        public string FatherName { get; set; }

        public string MotherName { get; set; }

        public string NationalCode { get; set; }


        public string BirthCertificateNo { get; set; }

        public string BirthCertificateMosalsal { get; set; }

        public string BirthCertificateDescription { get; set; }

        public DateTime Birthdate { get; set; }

        public int GenderTypeId { get; set; }

        public GenderType GenderType { get; set; }

        public int MarriageTypeId { get; set; }

        public MarriageType MarriageType { get; set; }

        public int? NumberOfChildren { get; set; }

        public int CityOfBirthId { get; set; }
        public City CityOfBirth { get; set; }

        public bool IsDisabled { get; set; }
        //پایان مشخصات فردی


        // مشخصات پرونده بهزیستی
        public long? MonthlyIncome { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        // تعداد معلولین در خانواده
        public int NumberOfDisabledInFamily { get; set; }

        public int? ClientTypeId { get; set; }

        public string ClientTypeDescription { get; set; }

        public ClientType ClientType { get; set; }

        public bool IsHouseHold { get; set; }

        //پایان مشخصات پرونده بهزیستی


        // مشخصات تماس

        public string WorkAddress { get; set; }

        public string HomeTel { get; set; }

        public string WorkTel { get; set; }

        public string Mobile { get; set; }

        public string EmergencyTel { get; set; }

        //پایان مشخصات تماس


        // اطلاعات مسکن فعلی


        public int HouseCityId { get; set; }
        public City HouseCity { get; set; }

        public string Address { get; set; }

        // محدوده استان فارس بر اساس طول و عرض جغرافیایی
        // البته این محدوده به صورت مستطیلی در نظر گرفته شده و خطا دارد

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string PostalCode { get; set; }

        public int CurrentHouseTypeId { get; set; }
        public CurrentHouseType CurrentHouseType { get; set; }

        public long? DepositAmount { get; set; }

        public long? MonthlyRentalRate { get; set; }

        public string RelativeFamilyNameWhoClientLiveInHerHouse { get; set; }

        public string OtherDescription { get; set; }

        //پایان اطلاعات مسکن فعلی


        public ICollection<ClientWaitingApplicantRequest> Requests { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// در صورتی که متقاضی تایید نشده بود این گزینه به معنی حذف شده می باشد
        /// </summary>
        public bool IsDeleted { get; set; }

        public ICollection<ClientWaitingApplicantLog> ClientWaitingApplicantLogs { get; set; }

    }
}