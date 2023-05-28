using System.Data.Entity;
using BehzistiMaskan.Core;
using BehzistiMaskan.Core.Configuration;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Models.FormBuilder;
using BehzistiMaskan.Core.Models.Geographic;
using BehzistiMaskan.Core.Models.ReportBuilder;
using BehzistiMaskan.Core.Models.Utility;
using BehzistiMaskan.Core.Utility;
using Microsoft.ApplicationInsights.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BehzistiMaskan.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("BehzistiMaskanConnection", false)
        {
        }

        //GeoGraphic
        public DbSet<Province> Provinces { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<RequiredMessage2> RequiredMessage2s { get; set; }

        //Type Utility
        public DbSet<FamilyRelationType> FamilyRelationTypes { get; set; }
        public DbSet<MarriageType> MarriageTypes { get; set; }
        public DbSet<ClientType> ClientTypes { get; set; }
        public DbSet<AssistanceType> AssistanceTypes { get; set; }
        public DbSet<CurrentHouseType> CurrentHouseTypes { get; set; }
        public DbSet<GenderType> GenderTypes { get; set; }
        public DbSet<BankType> BankTypes { get; set; }
        public DbSet<CoOrganizationType> CoOrganizationTypes { get; set; }
        public DbSet<BenefactorHelpType> BenefactorHelpTypes { get; set; }
        public DbSet<BenefactorHelpCategory> BenefactorHelpCategories { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<MaterialType> MaterialTypes { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }

        //Client Detail
        public DbSet<Person> Persons { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<FamilyRelation> FamilyRelations { get; set; }
        public DbSet<ClientDocument> ClientDocuments { get; set; }
        public DbSet<CurrentHousing> CurrentHousings { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<BankInfo> BankInfos { get; set; }

        public DbSet<RequestType> RequestTypes { get; set; }
        public DbSet<ClientRequest> ClientRequests { get; set; }
        public DbSet<ClientRequestGetLetter> ClientRequestGetLetters { get; set; }
        public DbSet<ClientExemptionBenefit> ClientExemptionBenefits { get; set; }

        public DbSet<ClientWaitingApplicant> ClientWaitingApplicants { get; set; }
        public DbSet<ClientWaitingApplicantRequest> ClientWaitingApplicantRequests { get; set; }

        public DbSet<ClientWaitingApplicantLog> ClientWaitingApplicantLogs { get; set; }

        public DbSet<ClientPhysicalProgress> ClientPhysicalProgresses { get; set; }
        public DbSet<ClientPhysicalProgressPhoto> ClientPhysicalProgressPhotos { get; set; }

        public DbSet<FinancialAid> FinancialAids { get; set; }

        public DbSet<ClientUser> ClientUsers { get; set; }

        public DbSet<ClientState> ClientStates { get; set; }
        public DbSet<ClientStateLog> ClientStateLogs { get; set; }

        public DbSet<ClientLog> ClientLogs { get; set; }

        public DbSet<DeletedClient> DeletedClients { get; set; }

        public DbSet<ClientRequiredMaterial> ClientRequiredMaterials { get; set; }

        public DbSet<DownloadRequest> DownloadRequests { get; set; }

        //Benefactor

        public DbSet<Benefactor> Benefactors { get; set; }
        public DbSet<BenefactorPayment> BenefactorPayments { get; set; }

        //Form Detail -- create form
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormMeta> FormMetas { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldTemplate> FieldTemplates { get; set; }
        public DbSet<FieldMeta> FieldMetas { get; set; }
        public DbSet<PhysicalProgress> PhysicalProgresses { get; set; }
        public DbSet<FormPhysicalProgress> FormPhysicalProgresses { get; set; }
        public DbSet<FormPhysicalProgressHelp> FormPhysicalProgressHelps { get; set; }
        public DbSet<FormAccessLevel> FormCountyAccessLevels { get; set; }
        public DbSet<FormCoOrganizationRole> FormCoOrganizationRoles { get; set; }

        public DbSet<ClientForm> ClientForms { get; set; }
        public DbSet<ClientFormField> ClientFormFields { get; set; }


        public DbSet<CoOrganizationApproveList> CoOrganizationApproveLists { get; set; }

        public DbSet<FormEmtiazBandi> FormEmtiazBandi { get; set; }

        //Report
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportCounty> ReportCounties { get; set; }

        public DbSet<ReportForm> ReportForms { get; set; }
        public DbSet<ReportFormField> ReportFormFields { get; set; }

        public DbSet<RequiredMessage> RequiredMessages { get; set; }


        //User Data
        public DbSet<UserInfo> UserInfos { get; set; }


        //Temporary Image (image store upload from client form filed data editor
        public DbSet<TemporaryImage> TemporaryImages { get; set; }

        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProvinceConfiguration());
            modelBuilder.Configurations.Add(new CountyConfiguration());
            modelBuilder.Configurations.Add(new DistrictConfiguration());
            modelBuilder.Configurations.Add(new CityConfiguration());

            modelBuilder.Configurations.Add(new PersonConfiguration());
            modelBuilder.Configurations.Add(new ClientConfiguration());
            modelBuilder.Configurations.Add(new ClientDocumentConfiguration());
            modelBuilder.Configurations.Add(new CurrentHousingConfiguration());
            modelBuilder.Configurations.Add(new FamilyRelationConfiguration());
            modelBuilder.Configurations.Add(new ContactInfoConfiguration());
            modelBuilder.Configurations.Add(new BankInfoConfiguration());
            modelBuilder.Configurations.Add(new ClientWaitingApplicantConfiguration());
            modelBuilder.Configurations.Add(new ClientUserConfiguration());
            modelBuilder.Configurations.Add(new RequestTypeConfiguration());
            modelBuilder.Configurations.Add(new MaterialTypeConfiguration());

            modelBuilder.Configurations.Add(new ClientWaitingApplicantRequestConfiguration());
            modelBuilder.Configurations.Add(new ClientRequestConfiguration());
            modelBuilder.Configurations.Add(new ClientRequestGetLetterConfiguration());
            modelBuilder.Configurations.Add(new ClientExemptionBenefitConfiguration());
            modelBuilder.Configurations.Add(new ClientPhysicalProgressConfiguration());
            modelBuilder.Configurations.Add(new ClientPhysicalProgressPhotoConfiguration());
            modelBuilder.Configurations.Add(new ClientRequiredMaterialConfiguration());
            modelBuilder.Configurations.Add(new RequiredMessage2Configuration());


            modelBuilder.Configurations.Add(new FormConfiguration());
            modelBuilder.Configurations.Add(new FormMetaConfiguration());
            modelBuilder.Configurations.Add(new FieldConfiguration());
            modelBuilder.Configurations.Add(new FieldMetaConfiguration());
            modelBuilder.Configurations.Add(new PhysicalProgressConfiguration());
            modelBuilder.Configurations.Add(new FormAccessLevelConfiguration());
            modelBuilder.Configurations.Add(new FormCoOrganizationRoleConfiguration());
            modelBuilder.Configurations.Add(new FormPhysicalProgressHelpConfiguration());
            modelBuilder.Configurations.Add(new FormPhysicalProgressConfiguration());

            modelBuilder.Configurations.Add(new BenefactorConfiguration());
            modelBuilder.Configurations.Add(new BenefactorPaymentConfiguration());

            modelBuilder.Configurations.Add(new UserInfoConfiguration());

            modelBuilder.Configurations.Add(new BankTypeConfiguration());

            modelBuilder.Configurations.Add(new DeletedClientConfiguration());

            modelBuilder.Configurations.Add(new ClientFormConfiguration());
            modelBuilder.Configurations.Add(new ClientFormFieldConfiguration());

            modelBuilder.Configurations.Add(new FinancialAidConfiguration());


            modelBuilder.Configurations.Add(new TemporaryImageConfiguration());

            modelBuilder.Configurations.Add(new ReportCountyConfiguration());

            modelBuilder.Configurations.Add(new ReportFormConfiguration());

            modelBuilder.Configurations.Add(new ReportFormFieldConfiguration());

            modelBuilder.Configurations.Add(new ClientStateConfiguration());
            modelBuilder.Configurations.Add(new ClientStateLogConfiguration());

            modelBuilder.Configurations.Add(new ClientLogConfiguration());
            modelBuilder.Configurations.Add(new CoOrganizationApproveListConfiguration());

            modelBuilder.Configurations.Add(new FormEmtiazBandiConfiguration());

            modelBuilder.Configurations.Add(new DownloadRequestConfiguration());

            modelBuilder.Configurations.Add(new SystemSettingConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}