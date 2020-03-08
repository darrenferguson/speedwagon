using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using SpeedWagon.Runtime.Extension;
using SpeedWagon.Web.Interfaces;
using SpeedWagon.Web.Models.ContentType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedWagon.Web.Services
{
    public class WebContentService : BaseSpeedWagonService, IWebContentService
    {
        private readonly IContentService _cachelessContentService;
        private readonly string _contentRoot;
        
        public override string Root => "content";

        public WebContentService(IContentService cachelessContentService, string contentRoot) : base(contentRoot)
        {
            this._cachelessContentService = cachelessContentService;
            this._contentRoot = contentRoot;
        }


        public async Task<SpeedWagonContent> GetContent(string path)
        {
            return await this._cachelessContentService.GetContent(RationalisePath(path));
        }


        public async Task<IEnumerable<SpeedWagonContent>> List(string path)
        {   
            return await this._cachelessContentService.Children(await GetContent(path));
        }

        public void Add(string parent, string name, string type, string user)
        {
            string urlName = RationalisePath(parent) + "/" + name.ToUrlName();

            SpeedWagonContent content = new SpeedWagonContent(name.ToTitleCasedName(), urlName, "content", user);
            string viewName = type.ToTitleCasedName();
            content.Template = viewName;
            content.Type = type;

            this._cachelessContentService.AddContent(content);
        }

        public void Save(SpeedWagonContent content, string user)
        {
            content.WriterName = user;
            content.UpdateDate = DateTime.Now;
     
            this._cachelessContentService.AddContent(content);
        }

        public IDictionary<string, string> GetValues(SpeedWagonContent content, IEnumerable<ContentTypeEditor> properties)
        {
            IDictionary<string, string> values = content.Content.ToDictionary(k => k.Key, k => k.Value == null ? string.Empty : k.Value.ToString());

            if(properties != null)
            {
                foreach (ContentTypeEditor contentTypeEditor in properties)
                {
                    if (!values.ContainsKey(contentTypeEditor.Name))
                    {
                        values.Add(contentTypeEditor.Name, string.Empty);
                    }
                }
            }

            return values;
        }

        public void SetValues(SpeedWagonContent content, IDictionary<string, string> values)
        {
            IDictionary<string, object> properties = content.Content;

            foreach (KeyValuePair<string, string> propertyValue in values)
            {
                if (properties.ContainsKey(propertyValue.Key))
                {
                    properties[propertyValue.Key] = propertyValue.Value;
                }
                else
                {
                    properties.Add(propertyValue.Key, propertyValue.Value);
                }
            }
            content.Content = properties;
        }
    }
}