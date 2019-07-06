using Microsoft.AspNetCore.Mvc.Rendering;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Enum;
using System;

namespace SpeedWagon.Web.Extension
{
    public static class HtmlHelperExtensions
    {

        ///<summary>
        /// Adds a partial view script to the Http context to be rendered in the parent view
        /// </summary>

        public static IHtmlHelper ClientRequirement(this IHtmlHelper htmlHelper, string file, ClientSideIncludeType type)
        {
            string key = "_speedwagon_" + type.ToString().ToLower() + "_" + file.ToUrlName();

            htmlHelper.ViewContext.HttpContext.Items[key] = file;

            return null;
        }

        ///<summary>
        /// Renders any scripts used within the partial views
        /// </summary>

        /// 
        public static IHtmlHelper RenderPartialViewScripts(this IHtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_speedwagon_javascript"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as string;
                    if (template != null)
                    {
                        if(!template.StartsWith("/"))
                        {
                            template = $"/js/speedwagon/editor/{template}";
                        }
                        htmlHelper.ViewContext.Writer.Write($"<script type=\"text/javascript\" src=\"{template}\"></script>" + Environment.NewLine);
                    }
                }
            }
            return null;
        }

        public static IHtmlHelper RenderPartialViewStyles(this IHtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_speedwagon_css"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as string;
                    if (template != null)
                    {
                        if (!template.StartsWith("/"))
                        {
                            template = $"/css/speedwagon/editor/{template}";
                        }
                        htmlHelper.ViewContext.Writer.Write($"<link rel=\"stylesheet\" href=\"{template}\" />" + Environment.NewLine);
                    }
                }
            }
            return null;
        }
    }
}