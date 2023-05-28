using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI;

namespace BehzistiMaskan.Core.Utility
{
    public static class UsefulMethods
    {
        public static string RenderPartialToString(this string controlName, object viewData)
        {
            var viewPage = new ViewPage
                {ViewContext = new ViewContext(), ViewData = new ViewDataDictionary(viewData)};

            viewPage.Controls.Add(viewPage.LoadControl(controlName));

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    viewPage.RenderControl(tw);
                }
            }

            return sb.ToString();
        }


        public static string GetFieldTemplatePath()
        {
            return $"{AppDomain.CurrentDomain.BaseDirectory}Core\\FieldTemplate\\";
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        //public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string memberName, bool asc = true)
        //{
        //    if (string.IsNullOrEmpty(memberName)) memberName = "Id";

        //    ParameterExpression[] typeParams = {Expression.Parameter(typeof(T), "")};

        //    var pi = typeof(T).GetProperty(memberName);

        //    return (IOrderedQueryable<T>) query.Provider.CreateQuery(
        //        Expression.Call(
        //            typeof(Queryable),
        //            asc ? "OrderBy" : "OrderByDescending",
        //            new[] {typeof(T), pi.PropertyType},
        //            query.Expression,
        //            Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
        //    );
        //}


        public static MvcHtmlString Disable(this MvcHtmlString helper, bool disabled) {
            var html = helper.ToString();
            var regex = new Regex("(disabled(?:=\".*\")?)");
            if (regex.IsMatch(html)) {
                html = regex.Replace(html, disabled ? "disabled=\"disabled\"" : "", 1);
            } else {
                regex = new Regex(@"(\/?>)");
                html = regex.Replace(html, disabled ? "disabled=\"disabled\"$1" : "$1", 1);
            }
            return MvcHtmlString.Create(html);
        }
    }
}