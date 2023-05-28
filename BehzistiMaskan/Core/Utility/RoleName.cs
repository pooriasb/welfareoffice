namespace BehzistiMaskan.Core.Utility
{
    public static class RoleName
    {
        //public const string Madadjoo = "Madadjoo";
        public const string CanManageClient = "CanManageClient";
        public const string KarshenasKeshvar = "KarshenasKeshvar";
        public const string KarshenasOstan = "KarshenasOstan";
        public const string KarshenasMasoolOstan = "KarshenasMasoolOstan";
        public const string KarshenasShahrestan = "KarshenasShahrestan";
        //public const string Modir = "Modir";
        //public const string Moaven = "Moaven";
        public const string MoavenMosharekat = "MoavenMosharekat";

        public const string ModirKolOstan = "ModirKolOstan";
        public const string ModirShahrestan = "ModirShahrestan";
        public const string SystemAdministrator = "SystemAdministrator";
        public const string SazmanHamkar = "SazmanHamkar";

        public const string MoavenOstan = "MoavenOstan";
        public const string KarmandOstan = "KarmandOstan";


        public static string ConvertRoleNameToPersian(this string roleName)
        {
            switch (roleName)
            {
                case KarshenasOstan:
                    return "کارشناس استان";
                case KarshenasShahrestan:
                    return "کارشناس شهرستان";
                case KarshenasKeshvar:
                    return "کارشناس کشور";
                case MoavenMosharekat:
                    return "معاون مشارکت ، مراکز و مسکن";
                //case Madadjoo:
                //    return "مددجو";
                //case Modir:
                //    return "معاونین";
                //case Moaven:
                //    return "مدیران";
                case SystemAdministrator:
                    return "مدیر سیستم";
                case CanManageClient:
                    return "مجوز تغییر مددجویان";
                case SazmanHamkar:
                    return "سازمان همکار";
                case KarshenasMasoolOstan:
                    return "کارشناس مسئول استان";
                case ModirKolOstan:
                    return "مدیر کل استان";
                case ModirShahrestan:
                    return "مدیر شهرستان";
                case MoavenOstan:
                    return "معاون استان";
                case KarmandOstan:
                    return "کارمند استان";
                default:
                    return roleName;
            }
        }
    }
}