using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AidatPro.Framework
{
    public static class HtmlHelper
    {
        #region Enum Helper
        public static MvcHtmlString EnumSelectListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            return EnumSelectListFor(htmlHelper, expression, null, htmlAttributes);
        }

        public static MvcHtmlString EnumSelectListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel, object htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = metadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(enumType);
            if (underlyingType != null)
                enumType = underlyingType;

            var values = from Enum value in Enum.GetValues(enumType)
                         select new
                         {
                             Id = value,
                             Name = value.GetType().Name
                         };

            return htmlHelper.DropDownListFor(expression, new SelectList(values, "Id", "Name", metadata.Model), optionLabel, htmlAttributes);
        }

        public static MvcHtmlString EnumDisplayNameFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var type = (Enum)metadata.Model;

            return new MvcHtmlString(type.GetType().Name);
        }
        #endregion

        #region Ckeditor
        /// <summary>
        /// Returns html to render Ckeditor with the specified form name
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="config">A Ckeditor config object which can contain any setting mentioned here: http://docs.ckeditor.com/#!/api/CKEDITOR.config e.g. new { height = 500 }</param>
        /// <returns></returns>
        public static MvcHtmlString Ckeditor(this System.Web.Mvc.HtmlHelper htmlHelper, string name, object config = null)
        {
            return htmlHelper.Editor(name, "Ckeditor", new { Config = config });
        }

        /// <summary>
        /// Returns html to render Ckeditor for the specified model property
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="config">A Ckeditor config object which can contain any setting mentioned here: http://docs.ckeditor.com/#!/api/CKEDITOR.config e.g. new { height = 500 }</param>
        /// <returns></returns>
        public static MvcHtmlString CkeditorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object config = null)
        {
            return htmlHelper.EditorFor(expression, "Ckeditor", new { Config = config });
        }
        #endregion

        public static string IsActive(this System.Web.Mvc.HtmlHelper html, string control)
        {
            var routeData = html.ViewContext.RouteData;

            //var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            // both must match
            var returnActive = control == routeControl;

            return returnActive ? "waves-effect active" : "";
        }

        public static string IsActiveAct(this System.Web.Mvc.HtmlHelper html, string action)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            //var routeControl = (string)routeData.Values["controller"];

            // both must match
            var returnActive = action == routeAction;

            return returnActive ? "active" : "";
        }


    }
}