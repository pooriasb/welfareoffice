using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Models;

namespace BehzistiMaskan.Core.Utility
{
    /// <summary>
    /// تنظیمات سامانه در این کلاس ذخیره می گردد. اطلاعات در جدول تنظیمات در پایگاه داده ذخیره می گردد
    /// </summary>
    public static class SystemSettingsManager
    {
        /// <summary>
        /// نشان می دهد که آیا تنظیمات سیستم مقدار دهی اولیه شده است یا خیر
        /// </summary>
        public static bool HasInitialized;

        /// <summary>
        /// فاصله زمانی مجاز بین دو درخواست دانلود اطلاعات مددجو بر حسب دقیقه
        /// پیش فرض: 10 دقیقه
        /// </summary>
        public static int DownloadRequestRestrictTimeOutInMinuets;

        public static ApplicationDbContext DbContext;

        /// <summary>
        /// مقدار دهی اولیه به تنظیمات و بارگذاری تنظیمات سیستمی از پایگاه داده
        /// </summary>
        public static void Initialize()
        {
            DownloadRequestRestrictTimeOutInMinuets = 10;

            if (DbContext == null)
                DbContext = new ApplicationDbContext();


            var allSettingsInDb = DbContext.SystemSettings.ToList();

            var settingDownloadRequestRestrictTimeOutInMinuets =
                allSettingsInDb.SingleOrDefault(s => s.Name == nameof(DownloadRequestRestrictTimeOutInMinuets));
            if (settingDownloadRequestRestrictTimeOutInMinuets != null)
            {
                int res;
                var canParseData = int.TryParse(settingDownloadRequestRestrictTimeOutInMinuets.Value, out res);
                if (canParseData)
                    DownloadRequestRestrictTimeOutInMinuets = res;
            }

            HasInitialized = true;
        }

        /// <summary>
        /// همان تابع مقدار دهی اولیه را فراخوانی می کند
        /// </summary>
        public static void RefreshSettings()
        {
            Initialize();
        }
    }
}