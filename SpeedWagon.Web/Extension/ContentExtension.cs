using Newtonsoft.Json;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            if(model.Content[key] == null) {
                return string.Empty;
            }

            return model.Content[key].ToString();
        }

        public static T GetValue<T>(this SpeedWagonContent model, string key)
        {
            var v = model.GetValue(key);

            if (v == null || !CanChangeType(v, typeof (T)))
                return (T) Activator.CreateInstance(typeof(T));

            try
            {
                return (T)Convert.ChangeType(v, typeof(T));
            } catch(Exception ex)
            {
                return JsonConvert.DeserializeObject<T>(v);
            }
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

        public async static Task<SpeedWagonContent> Home(this IContentService contentService, SpeedWagonContent model)
        {
            return await contentService.Home(model);
        }

        public async static Task<IEnumerable<SpeedWagonContent>> TopNavigation(this IContentService contentService, SpeedWagonContent model)
        {
            return await contentService.TopNavigation(model);
        }

        public async static Task<IEnumerable<SpeedWagonContent>> Descendants(this IContentService contentService, SpeedWagonContent model)
        {
            return await contentService.Descendants(model);
        }
        
        public async static Task<IEnumerable<SpeedWagonContent>> Descendants(this IContentService contentService, SpeedWagonContent model, string type)
        {
            IEnumerable<SpeedWagonContent> content = await contentService.Descendants(model);
            return content.Where(item => item.Type == type);
        }

        public async static Task<IEnumerable<SpeedWagonContent>> Descendants(this IContentService contentService, SpeedWagonContent model, IDictionary<string, string> filters)
        {
            return await contentService.Descendants(model, filters);
        }

        public static async Task<IEnumerable<SpeedWagonContent>> Children(this IContentService contentService, SpeedWagonContent model)
        {
            return await contentService.Children(model);
        }

        public static string View(this SpeedWagonContent model)
        {
            return "~/Views/" + model.Template + ".cshtml";
        }
    }
}
