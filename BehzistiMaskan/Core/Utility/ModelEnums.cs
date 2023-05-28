
using System;
using System.Data;
using BehzistiMaskan.Core.Models;

namespace BehzistiMaskan.Core.Utility
{
    public static class ModelEnums
    {
        public enum CoOrganizationType
        {
            BonyadMaskan = 1,
            RahoShahrsazi = 2,
            AnjomanKhayerin = 3,
            BonyadMostazafan = 4,
            SepahPasdaran = 5,
            BasijSazandegi = 6,
            GharargahMahdoodiadZodaei = 7,
            EdareyeAb = 8,
            EdareyeBargh = 9,
            EdareyeGas = 10,
            ShahrdariVaDehdari = 11,
            BoyadBarekat = 12,
            GharargahSazandegi = 13,
            AfradKhayer = 14,
            Sayer = 15
        }

        public enum FieldTemplateE
        {
            TextBoxTemplate = 1,
            NumberTemplate = 2,
            DateTemplate = 3,
            ImageTemplate = 4,
            HeaderSmallTemplate = 5,
            HeaderMediumTemplate = 6,
            HeaderBigTemplate = 7,
            SplitterTemplate = 8
        }

        public enum DocumentTypesE
        {
            ShenasnamePage1 = 1,
            ShenasnamePage2 = 2,
            ShenasnamePageOther = 3,
            NationalCardFront = 4,
            NationalCardBack = 5,
            MarriageDocument = 6,
            HelperReport = 7,
            HomeContract = 8,
            ProvingForBenefactor = 9,
            FormFieldDocument = 10,
            PhysicalProgressPhoto = 11,
            PhysicalProgressReport = 12,
            FinancialAidBankFish = 13,
            FormEmtiazBandi = 14,
            BankTaeidHesabOffline = 15,
            /// <summary>
            /// معرفی نامه معافیت انشعابات
            /// </summary>
            RequestGetLetter = 16,
            /// <summary>
            /// نامه بهره مندی از معافیت انشعابات
            /// </summary>
            RequestExemptionBenefit = 17,
        }

        public enum CurrentHouseTypeE
        {
            Ejarei = 1,
            Bastegan = 2,
            Khayerin = 3,
            Sayer = 4
        }

        public enum StatusTypeE
        {
            Success,
            Error,
            Warning,
            ValidationError,
            Custom,
            DuplicateNationalCode,
            DeletedSuccessful,
            YouCannotAccessThisArea,
            YouCannotEditClient,
            RestrictionOnDownloadRequest
        }

        public enum ClientRequestTypeE
        {
            FixHome = 1,
            BuildAHouseLanded = 2,
            BuildAHouseCanBuyLand = 3,
            BuyAHouse = 4,
            ExemptionWater = 5,
            ExemptionElectrical = 6,
            ExemptionGas = 7,
            ExemptionProductionLicense = 8,
        }

        public enum MaterialTypeE
        {
            Girder = 1, //تیرآهن
            Rebar = 2, //میلگرد
            Cement = 3,
            Plaster = 4,
            Brick = 5,
            Block = 6,
            Tile = 7,
            Door = 8,
            Window = 9
        }


        public enum PhysicalProgressTypeE
        {

            VagozariZamin = 1,
            SodoorParvaneSaakht = 2,
            GharardadBaBankAamel = 3,
            PaySaazi = 4,
            EjrayeEskelet = 5,
            EjrayeDivaroSaghf = 6,
            SeftKari = 7,
            NazokKari = 8,
            VagozarShodeh = 9,
        }
        public enum ClientStateTypeE
        {
            // فرد متقاضی مددجوی بهزیستی نمی باشد
            IsIllegalPersonAndNotABehzistiClient = -1,

            // ******************************************************************
            // ------------------------ مددجو ------------------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // در انتظار تکمیل اطلاعات توسط مددجو InWaitingListClientToCompleteData
            // ثبت اطلاعات جدید توسط مددجو SubmitNewDataByClient
            // وضعیت در صف انتظار بررسی توسط کارشناس شهرستان InWaitingListKarshenasShahrestan
            // وضعیت بازگشت توسط کارشناس شهرستان DenyByKarshenasShahrestan
            // وضعیت در صف ثبت گزارش پیشرفت فیزیکی جدید InWaitingListPhysicalProgress
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // همیشه امکان مشاهده اطلاعات و وضعیت فعلی پرونده خود را دارد
            // ******************************************************************

            // وضعیت ثبت نام اولیه توسط خود مددجو
            InitialClientRegister = 0,

            // در انتظار تکمیل اطلاعات توسط مددجو
            InWaitingListClientToCompleteData = 1,

            // وقتی که اطلاعات مددجو به دلیل
            // مغایرت در اطلاعات
            // و یا نقص اطلاعات
            // توسط کارشناس شهرستان بازگشت خورده باشد
            // مددجو دوباره می تواند گزینه تایید و ارسال نهایی برای کارشناس شهرستان را بزند
            // و در این وضعیت قرار داده می شود
            SubmitNewDataByClient = 2,

            //ثبت شده در سامانه توسط بارگذاری اولیه اطلاعات مددجویان قبلی
            RegisterByInitialClientImport = 3,

            // ******************************************************************
            // ------------------------ کارشناس شهرستان ------------------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // همه موارد کارتابل و وضعیت های
            // در انتظار تکمیل اطلاعات توسط مددجو InWaitingListClientToCompleteData
            // وضعیت در صف انتظار بررسی توسط کارشناس استان InWaitingListKarshenasOstan
            // وضعیت بازگشت توسط کارشناس شهرستان DenyByKarshenasShahrestan
            // وضعیت در صف ثبت گزارش پیشرفت فیزیکی جدید InWaitingListPhysicalProgress
            // 
            // ++++++++ کارتابل ++++++++
            // ویرایش و مشاهده در کارتابل فقط در وضعیت های زیر
            // ثبت نام اولیه توسط خود مددجو InitialClientRegister
            // ثبت دوباره اطلاعات توسط مددجو SubmitNewDataByClient
            // وضعیت در صف انتظار بررسی توسط کارشناس شهرستان InWaitingListKarshenasShahrestan
            // وضعیت در صف انتظار تایید سازمان همکار شهرستان InWaitingListSazmanHamkarShahrestan
            // وضعیت عدم تایید توسط کارشناس سازمان همکار شهرستان DenyBySazmanHamkarShahrestan
            // وضعیت تایید شده توسط تمامی سازمان های همکار شهرستان در آن طرح ApproveByAllOfSazmanHamkarShahrestan
            // وضعیت عدم تایید و عودت توسط کارشناس استان DenyByKarshenasOstan
            // وضعیت در انتظار تایید فرم امتیاز بندی InWaitingListFormEmtiazBandi
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // کارشناس شهرستان اطلاعات تمامی مددجویان ثبت شده در آن شهرستان را می تواند مشاهده کند
            // ******************************************************************



            // ******************************************************************
            // ------------------------ مدیر شهرستان ------------------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // در هیج مرحله ای امکان ویرایش اطلاعات را ندارد
            // 
            // ++++++++ کارتابل ++++++++
            // وضعیت در انتظار تایید فرم امتیاز بندی InWaitingListFormEmtiazBandi
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // مدیر شهرستان اطلاعات تمامی مددجویان ثبت شده در آن شهرستان را می تواند مشاهده کند
            // ******************************************************************

            // در صف انتظار بررسی توسط کارشناس شهرستان
            InWaitingListKarshenasShahrestan = 4,

            // عودت یا بازگشت توسط کارشناس شهرستان
            DenyByKarshenasShahrestan = 5,

            // ******************************************************************
            // ------------------------ کاربر سازمان همکار شهرستان ------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // در هیچ وضعیتی نمی تواند اطلاعات مددجو را ویرایش کند
            //
            // ++++++++ کارتابل ++++++++
            // فقط در این وضعیت ها اطلاعات در کارتابل او نمایش داده خواهد شد 
            // در صف انتظار بررسی توسط کاربر سازمان همکار شهرستان InWaitingListSazmanHamkarShahrestan
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // تنها می تواند مددجویان شهرستان خود که در طرحی ثبت شده اند که آن سازمان وی به عنوان همکار در طرح ثبت شده باشد را مشاهده کند
            // ******************************************************************

            // در صف انتظار بررسی توسط کاربر سازمان همکار شهرستان
            InWaitingListSazmanHamkarShahrestan = 6,

            // عدم تایید یا عودت توسط کاربر سازمان همکار شهرستان
            DenyBySazmanHamkarShahrestan = 7,

            ApproveBySazmanHamkarShahrestan = 70,

            // تایید شده توسط تمامی کارشناسان شهرستان سازمان های همکار در طرح انتخاب شده برای مددجو
            ApproveByAllOfSazmanHamkarShahrestan = 8,

            // ******************************************************************
            // ------------------------ کارشناس استان --------------------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // همیشه می تواند مددجویان استان خود را ویرایش کند
            //
            //

            // در تمامی وضعیت هایی که در کارتابل وی قرار دارد و همجنین در وضعیت های زیر
            // وضعیت برگشت توسط کارشناس استان DenyByKarshenasOstan
            // وضعیت در صف ثبت گزارش پیشرفت فیزیکی جدید InWaitingListPhysicalProgress
            // وضعیت در صف پرداخت کمک بلاعوض InWaitingListForPay
            // در مرحله ثبت نام اولیه
            // در مرحله انتظار تکمیل اطلاعات توسط مددجو
            // در مرحله ثبت توسط بارگذاری اولیه اطلاعات
            //

            // ++++++++ کارتابل ++++++++
            // وضعیت در صف انتظار بررسی توسط کارشناس استان InWaitingListKarshenasOstan
            // وضعیت در صف انتظار تایید سازمان همکار استان InWaitingListSazmanHamkarOstan
            // وضعیت عدم تایید توسط سازمان همکار استان DenyBySazmanHamkarOstan
            // در وضعیت تایید توسط تمام سازمان های همکار استان ApproveByAllOfSazmanHamkarOstan
            // در وضعیت صف تایید فرم امتیاز بندی InWaitingListFormEmtiazBandi
            // وضعیت تایید توسط تمامی امضا کنندگان فرم امتیاز بندی ApproveAllFormEmtiazBandi
            // و وضعیت بازگشت توسط کارشناس کشور DenyByKarshenasOstan
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // در تمامی مراحل امکان مشاهده اطلاعات کلیه مددجویان استان خود را دارد
            // ******************************************************************



            // ******************************************************************
            // ------------------------ معاون مشارکت های استان ------------------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // در هیج مرحله ای امکان ویرایش اطلاعات را ندارد
            // 
            // ++++++++ کارتابل ++++++++
            // وضعیت در انتظار تایید فرم امتیاز بندی InWaitingListFormEmtiazBandi
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // معاون مشارکت های استان اطلاعات تمامی مددجویان ثبت شده در آن استان را می تواند مشاهده کند
            // ******************************************************************



            // ******************************************************************
            // ------------------------ سایر معاونین استان ---------------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // در هیج مرحله ای امکان ویرایش اطلاعات را ندارد
            // 
            // ++++++++ کارتابل ++++++++
            // وضعیت در انتظار تایید فرم امتیاز بندی InWaitingListFormEmtiazBandi
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // معاونین استان می توانند لیست مددجویانی که در معاونت آنها هستن را مشاهده کنند
            // ******************************************************************




            // ******************************************************************
            // ------------------------ مدیر کل استان --------------------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // در هیج مرحله ای امکان ویرایش اطلاعات را ندارد
            // 
            // ++++++++ کارتابل ++++++++
            // وضعیت در انتظار تایید فرم امتیاز بندی InWaitingListFormEmtiazBandi
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // می تواند اطلاعات تمامی مددجویان آن استان را مشاهده نماید
            // ******************************************************************



            // در صف انتظار بررسی توسط کارشناس استان
            InWaitingListKarshenasOstan = 9,

            // عودت یا بازگشت توسط کارشناس استان
            DenyByKarshenasOstan = 10,

            // ******************************************************************
            // ------------------------ کاربر سازمان همکار استان --------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // در هیچ وضعیتی نمی تواند اطلاعات مددجو را ویرایش کند
            //
            // ++++++++ کارتابل ++++++++
            // در صف انتظار بررسی توسط کاربر سازمان همکار استان InWaitingListSazmanHamkarOstan
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // تنها می تواند مددجویان استان خود را مشاهده کند که در طرحی ثبت شده اند که آن سازمان به عنوان همکار در آن طرح ثبت شده باشد
            // ******************************************************************


            // در صف انتظار بررسی توسط کاربر سازمان همکار در سطح استان
            InWaitingListSazmanHamkarOstan = 11,

            // عدم تایید یا عودت توسط کاربر سازمان همکار شهرستان
            DenyBySazmanHamkarOstan = 12,

            ApproveBySazmanHamkarOstan = 120,

            // تایید شده توسط تمامی کارشناسان استان سازمان های همکار در طرح انتخاب شده برای مددجو
            ApproveByAllOfSazmanHamkarOstan = 13,

            // نیازمند تایید فرم امتیاز بندی
            InWaitingListFormEmtiazBandi = 14,

            // تایید و امضا شده توسط تمام کسانی که باید فرم امتیاز بندی را امضا کنند
            ApproveAllFormEmtiazBandi = 15,


            // ******************************************************************
            // ------------------------ کارشناس کشور --------------------------
            //
            // ++++++++ ویرایش اطلاعات ++++++++
            // در هیچ وضعیتی امکان ویرایش اطلاعات را نخواهد داشت
            //
            // ++++++++ کارتابل ++++++++
            // وضعیت در صف انتظار بررسی توسط کارشناس کشور InWaitingListKarshenasKeshvar
            // وضعیت در لیست مشمولین دریافت کمک بلاعوض InWaitingListForPay
            //
            // ++++++++ مشاهده اطلاعات ++++++++
            // در تمامی مراحل امکان مشاهده کامل اطلاعات مددجویان کل کشور را دارد
            // ******************************************************************

            // در صف انتظار بررسی کارشناس کشور
            InWaitingListKarshenasKeshvar = 16,

            // بازگشت و عودت توسط کارشناس کشور
            DenyByKarshenasKeshvar = 17,

            // در صف ثبت گزارش پیشرفت فیزیکی
            InWaitingListPhysicalProgress = 18,

            // در صف پرداخت کمک بلاعوض - یا در لیست مشمولین دریافت کمک بلاعوض
            InWaitingListForPay = 19,

            // پرداخت به صورت کامل انجام شده است
            CompletePayHelpMoney = 20,

        }

        public static string ToPersianString(this ClientStateTypeE clientState)
        {
            switch (clientState)
            {
                case ModelEnums.ClientStateTypeE.IsIllegalPersonAndNotABehzistiClient:
                    return "مددجوی بهزیستی نمی باشد";

                case ModelEnums.ClientStateTypeE.InitialClientRegister:
                    return "ثبت نام اولیه";

                case ModelEnums.ClientStateTypeE.InWaitingListClientToCompleteData:
                    return "در انتظار تکمیل اطلاعات توسط مددجو";

                case ModelEnums.ClientStateTypeE.SubmitNewDataByClient:
                    return "بروزرسانی اطلاعات توسط مددجو";

                case ModelEnums.ClientStateTypeE.RegisterByInitialClientImport:
                    return "ثبت نام توسط ورود اولیه مددجویان";

                case ModelEnums.ClientStateTypeE.ApproveBySazmanHamkarShahrestan:
                    return "تایید شده توسط سازمان همکار شهرستان و در صف تایید بقیه سازمان های همکار";

                case ModelEnums.ClientStateTypeE.ApproveBySazmanHamkarOstan:
                    return "تایید شده توسط سازمان همکار استان و در صف تایید بقیه سازمان های همکار";

                case ModelEnums.ClientStateTypeE.InWaitingListKarshenasShahrestan:
                    return "در انتظار بررسی کارشناس شهرستان";

                case ModelEnums.ClientStateTypeE.DenyByKarshenasShahrestan:
                    return "برگشت یا عدم تایید توسط کارشناس شهرستان";

                case ModelEnums.ClientStateTypeE.InWaitingListSazmanHamkarShahrestan:
                    return "در انتظار تایید سازمان همکار شهرستان";

                case ModelEnums.ClientStateTypeE.DenyBySazmanHamkarShahrestan:
                    return "برگشت یا عدم تایید توسط سازمان همکار شهرستان";

                case ModelEnums.ClientStateTypeE.ApproveByAllOfSazmanHamkarShahrestan:
                    return "تایید شده توسط همه سازمان های همکار شهرستان";

                case ModelEnums.ClientStateTypeE.InWaitingListKarshenasOstan:
                    return "در انتظار بررسی کارشناس استان";

                case ModelEnums.ClientStateTypeE.DenyByKarshenasOstan:
                    return "برگشت یا عدم تایید توسط کارشناس استان";

                case ModelEnums.ClientStateTypeE.InWaitingListSazmanHamkarOstan:
                    return "در انتظار تایید سازمان همکار استان";

                case ModelEnums.ClientStateTypeE.DenyBySazmanHamkarOstan:
                    return "برگشت یا عدم تایید توسط سازمان همکار استان";

                case ModelEnums.ClientStateTypeE.ApproveByAllOfSazmanHamkarOstan:
                    return "تایید شده توسط همه سازمان های همکار استان";

                case ModelEnums.ClientStateTypeE.InWaitingListFormEmtiazBandi:
                    return "در انتظار تایید و امضای فرم امتیاز بندی";

                case ModelEnums.ClientStateTypeE.ApproveAllFormEmtiazBandi:
                    return "تایید کامل فرم امتیاز بندی";

                case ModelEnums.ClientStateTypeE.InWaitingListKarshenasKeshvar:
                    return "در انتظار بررسی کارشناس کشور";

                case ModelEnums.ClientStateTypeE.DenyByKarshenasKeshvar:
                    return "برگشت یا عدم تایید توسط کارشناس کشور";

                case ModelEnums.ClientStateTypeE.InWaitingListPhysicalProgress:
                    return "در انتظار تکمیل پیشرفت فیزیکی";

                case ModelEnums.ClientStateTypeE.InWaitingListForPay:
                    return "در انتظار پرداخت کمک بلاعوض";

                case ModelEnums.ClientStateTypeE.CompletePayHelpMoney:
                    return "پرداخت کامل کمک بلاعوض";
                default:
                    break;
            }

            return "";
        }

        //public static string ToPersianString(this DocumentTypesE documentType)
        //{
        //    switch (documentType)
        //    {
        //        case DocumentTypesE.ShenasnamePage1:
        //            return "صفحه اول شناسنامه";
        //        case DocumentTypesE.ShenasnamePage2:
        //            return "صفحه دوم شناسنامه";
        //        case DocumentTypesE.ShenasnamePageOther:
        //            return "سایر صفحات شناسنامه";
        //        case DocumentTypesE.NationalCardFront:
        //            return "کارت ملی (روی کارت)";
        //        case DocumentTypesE.NationalCardBack:
        //            return "کارت ملی (پشت کارت)";
        //        case DocumentTypesE.MarriageDocument:
        //            return "سند ازدواج";
        //        case DocumentTypesE.HelperReport:
        //            return "گزارش مددکار";
        //        case DocumentTypesE.HomeContract:
        //            return "قولنامه منزل";
        //        case DocumentTypesE.ProvingForBenefactor:
        //            return "مدرک اثبات اسکان در منزل خیر ";
        //        //case DocumentTypesE.FormFieldDocument:
        //        //    break;
        //        //case DocumentTypesE.PhysicalProgressPhoto:
        //        //    break;
        //        //case DocumentTypesE.PhysicalProgressReport:
        //        //    break;
        //        //case DocumentTypesE.FinancialAidBankFish:
        //        //    break;
        //        //case DocumentTypesE.FormEmtiazBandi:
        //        //    break;
        //        //case DocumentTypesE.BankTaeidHesabOffline:
        //        //    break;
        //        //case DocumentTypesE.RequestGetLetter:
        //        //    break;
        //        //case DocumentTypesE.RequestExemptionBenefit:
        //        //    break;
        //        default:
        //            break;
        //    }
        //    return " --- ";

        //}
    }

    public static class FormStatusType
    {
        public const string Draft = "Draft";
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Complete = "Complete";
    }


    public static class ClientRequestTypeStr
    {
        public const string FixHome = "FixHome";
        public const string BuildAHouseLanded = "BuildAHouseLanded";
        public const string BuildAHouseCanBuyLand = "BuildAHouseCanBuyLand";
        public const string BuyAHouse = "BuyAHouse";
        public const string ExemptionWater = "ExemptionWater";
        public const string ExemptionGas = "ExemptionGas";
        public const string ExemptionElectrical = "ExemptionElectrical";
        public const string ExemptionProductionLicense = "ExemptionProductionLicense";
    }


    //public static class ClientStateType
    //{
    //    //ثبت نام اولیه
    //    public const string InitialRegister = "InitialClientRegister";
    //    // در صف انتظار بررسی توسط کارشناس شهرستان
    //    public const string InWaitingListKarshenasShahrestan = "InWaitingListKarshenasShahrestan";
    //    // عودت یا بازگشت توسط کارشناس شهرستان
    //    public const string DenyByKarshenasShahrestan = "DenyByKarshenasShahrestan";

    //    // در صف انتظار بررسی توسط کاربر سازمان همکار شهرستان
    //    public const string InWaitingListSazmanHamkarShahrestan = "InWaitingListSazmanHamkarShahrestan";
    //    // عدم تایید یا عودت توسط کاربر سازمان همکار شهرستان
    //    public const string DenyBySazmanHamkarShahrestan = "DenyBySazmanHamkarShahrestan";

    //    // در صف انتظار بررسی توسط کارشناس استان
    //    public const string InWaitingListKarshenasOstan = "InWaitingListKarshenasOstan";
    //    // عودت یا بازگشت توسط کارشناس استان
    //    public const string DenyByKarshenasOstan = "DenyByKarshenasOstan";

    //    // در صف انتظار بررسی توسط کاربر سازمان همکار در سطح استان
    //    public const string InWaitingListSazmanHamkarOstan = "InWaitingListSazmanHamkarOstan";
    //    // عدم تایید یا عودت توسط کاربر سازمان همکار شهرستان
    //    public const string DenyBySazmanHamkarOstan = "DenyBySazmanHamkarOstan";

    //    // نیازمند تایید فرم امتیاز بندی
    //    public const string InWaitingListFormEmtiazBandi = "InWaitingListFormEmtiazBandi";

    //    // در صف انتظار بررسی کارشناس کشور
    //    public const string InWaitingListKarshenasKeshvar = "InWaitingListKarshenasKeshvar";
    //    // بازگشت و عودت توسط کارشناس کشور
    //    public const string DenyByKarshenasKeshvar = "DenyByKarshenasKeshvar";

    //    // در صف ثبت گزارش پیشرفت فیزیکی
    //    public const string InWaitingListPhysicalProgress = "InWaitingListPhysicalProgress";

    //    // در صف پرداخت کمک بلاعوض - یا در لیست مشمولین دریافت کمک بلاعوض
    //    public const string InWaitingListForPay = "InWaitingListForPay";

    //    // پرداخت به صورت کامل انجام شده است
    //    public const string CompletePayHelpMoney = "CompletePayHelpMoney";

    //    // فرد متقاضی مددجوی بهزیستی نمی باشد
    //    public const string IsIllegalPerson = "IsIllegalPerson";
    //}

}