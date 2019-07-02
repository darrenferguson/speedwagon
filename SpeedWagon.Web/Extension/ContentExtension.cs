using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeedWagon.Web.Extension
{
    public static class ContentExtension
    {
        public static string WebUrl(this SpeedWagonContent content)
        {
            string[] segments = content.RelativeUrl.Split('/');

            if(segments.Length < 3)
            {
                return content.RelativeUrl;
            }

            return "/" + string.Join("/", segments.Skip(3));
        }

        public static string GetValue(this SpeedWagonContent model, string key)
        {
            if (model == null || model.Content == null || !model.Content.ContainsKey(key))
                return string.Empty;

            return model.Content[key].ToString();
        }

        public static T GetValue<T>(this SpeedWagonContent model, string key)
        {
            var v = model.GetValue(key);

            if (v == null || !CanChangeType(v, typeof (T)))
                return (T) Activator.CreateInstance(typeof(T));

            return (T) Convert.ChangeType(v, typeof(T));
        }

        private static bool CanChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            var convertible = value as IConvertible;

            return convertible != null;
        }

        public static bool HideInNavigation(this SpeedWagonContent model)
        {
            return model.GetValue<bool>("HideInNavigation");
        }

        public static SpeedWagonContent Home(this IContentService contentService, SpeedWagonContent model)
        {
            return contentService.Home(model);
        }

        public static IEnumerable<SpeedWagonContent> TopNavigation(this IContentService contentService, SpeedWagonContent model)
        {
            return contentService.TopNavigation(model);
        }

        public static IEnumerable<SpeedWagonContent> Descendants(this IContentService contentService, SpeedWagonContent model)
        {
            return contentService.Descendants(model);
        }
        
        public static IEnumerable<SpeedWagonContent> Descendants(this IContentService contentService, SpeedWagonContent model, string type)
        {
            return contentService.Descendants(model).Where(item => item.Type == type);
        }

        public static IEnumerable<SpeedWagonContent> Descendants(this IContentService contentService, SpeedWagonContent model, IDictionary<string, string> filters)
        {
            return contentService.Descendants(model, filters);
        }

        public static IEnumerable<SpeedWagonContent> Children(this IContentService contentService, SpeedWagonContent model)
        {
            return contentService.Children(model);
        }

        public static string View(this SpeedWagonContent model)
        {
            return "~/Views/" + model.Template + ".cshtml";
        }
    }
}
