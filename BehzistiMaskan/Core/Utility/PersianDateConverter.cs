using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using AutoMapper;
using BehzistiMaskan.Core.Models.FormBuilder;
using MD.PersianDateTime;

namespace BehzistiMaskan.Core.Utility
{
    public class PersianToGregorianDateConverter : IValueConverter<PersianDateTime?, DateTime?>
    {
        public DateTime? Convert(PersianDateTime? sourceMember, ResolutionContext context)
        {
            return sourceMember?.ToDateTime();
        }
    }

    public class PersianStringDateToGregorianDateConverter : IValueConverter<string, DateTime?>
    {
        public DateTime? Convert(string sourceMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember)) return null;
            var persianDate = PersianDateTime.Parse(sourceMember);
            return persianDate.ToDateTime();
        }
    }

    public class GregorianToPersianDateConverter : IValueConverter<DateTime?, PersianDateTime?>
    {
        public PersianDateTime? Convert(DateTime? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return null;
            return new PersianDateTime(sourceMember);
        }
    }

    public class GregorianToPersianStringDateConverter : IValueConverter<DateTime?, string>
    {
        public string Convert(DateTime? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null) return "";
            return new PersianDateTime(sourceMember).ToString("yyyy/MM/dd");
        }
    }

    public static class ConvertDate
    {
        /// <summary>
        /// فقط در مواردی استفاده شود که مطمئن هستیم تاریخ نال نمی باشد. در غیر اینصورت با خطا مواجه می شویم
        /// </summary>
        /// <param name="source">ورودی که باید غیر نال باشد</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this PersianDateTime? source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return ((PersianDateTime)source).ToDateTime();
        }

        //    public static DateTime ToGregorian(this PersianDateTime source)
        //    {
        //        //return new PersianCalendar().ToDateTime(source.Year, source.Month, source.Day, 0, 0, 0, 0);
        //        return new DateTime(source.Year, source.Month, source.Day, new PersianCalendar());
        //    }

        public static PersianDateTime ToJalali(this DateTime source)
        {
            var pc = new PersianCalendar();
            var year = pc.GetYear(source);
            var month = pc.GetMonth(source);
            var day = pc.GetDayOfMonth(source);
            return new PersianDateTime(year, month, day);
        }

        //    public static PersianDateTime ToPersianDateTime(this DateTime source)
        //    {
        //        return new PersianDateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, source.Second);
        //    }

        //    public static DateTime ToDateTime(this PersianDateTime source)
        //    {
        //        return new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, source.Second);
        //    }
    }

    public class FormAccessLevelToString : IValueConverter<ICollection<FormAccessLevel>, string>
    {
        public string Convert(ICollection<FormAccessLevel> sourceAccessLevels, ResolutionContext context)
        {
            return sourceAccessLevels.Aggregate("", (current, formAccessLevel) => current + (formAccessLevel.County.Name + ", "));
        }
    }
}