using System;
using System.Collections.Generic;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;

namespace BehzistiMaskan.Core.Models
{
    public class Client
    {
        public Client()
        {
            ClientDocuments = new HashSet<ClientDocument>();
            ClientForms = new HashSet<ClientForm>();
        }

        public int Id { get; set; }

        public Person Person { get; set; }

        public ICollection<ClientDocument> ClientDocuments { get; set; }
        public ICollection<FinancialAid> FinancialAids { get; set; }
        public ICollection<CoOrganizationApproveList> CoOrganizationApproveLists { get; set; }

        public ICollection<ClientLog> ClientLogs { get; set; }

        public CurrentHousing CurrentHousing { get; set; }

        public ContactInfo ContactInfo { get; set; }

        public BankInfo BankInfo { get; set; }

        public bool? IsHouseHold { get; set; }

        public int? ClientTypeId { get; set; }

        public ClientType ClientType { get; set; }

        public string ClientTypeDescription { get; set; }

        public int? AssistanceTypeId { get; set; }

        public AssistanceType AssistanceType { get; set; }

        public long? MonthlyIncome { get; set; }

        public bool? IsIncludedInComprehensiveReport { get; set; }

        public bool? IsDeleted { get; set; }

        public int CityId { get; set; }

        public City City { get; set; }

        // شماره پرونده مددجو در سامانه کشوری بهزیستی
        public string GlobalBehzistiUiCode { get; set; }

        // تعداد معلولین در خانواده
        public int? NumberOfDisabledInFamily { get; set; }

        // شماره پرونده بهزیستی
        public string BehzistiCode { get; set; }

        public ICollection<ClientRequest> ClientRequests { get; set; }

        public ICollection<ClientForm> ClientForms { get; set; }

        public ICollection<TemporaryImage> TemporaryImages { get; set; }
        public ICollection<ClientPhysicalProgress> ClientPhysicalProgresses { get; set; }

        public DeletedClient DeletedClient { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }


        public ClientState ClientState { get; set; }

        public FormEmtiazBandi FormEmtiazBandi { get; set; }

        public ICollection<ClientStateLog> ClientStateLogs { get; set; }

        public ICollection<ClientRequiredMaterial> ClientRequiredMaterials { get; set; }

        public ICollection<DownloadRequest> DownloadRequests { get; set; }
    }
}