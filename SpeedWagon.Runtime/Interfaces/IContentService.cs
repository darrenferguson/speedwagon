using SpeedWagon.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeedWagon.Interfaces
{
    public delegate void ContentAddedHandler(SpeedWagonContent sender, EventArgs e);
    public delegate void ContentRemovedHandler(string sender, EventArgs e);

    public interface IContentService
    {
        event ContentAddedHandler Added;
        event ContentRemovedHandler Removed;

        void AddContent(SpeedWagonContent model);
        void RemoveContent(string url);

        Task RefreshUrls();

        IEnumerable<string> GetUrlList();

        Task<SpeedWagonContent> GetContent(string url);

        Task<SpeedWagonContent> Home(SpeedWagonContent model);

        Task<IEnumerable<SpeedWagonContent>> TopNavigation(SpeedWagonContent model);
        Task<IEnumerable<SpeedWagonContent>> Children(SpeedWagonContent model);

        Task<IEnumerable<SpeedWagonContent>> Descendants(SpeedWagonContent model);

        Task<IEnumerable<SpeedWagonContent>> Descendants(SpeedWagonContent model, IDictionary<string, string> filter);

        Task<SpeedWagonContent> Parent(SpeedWagonContent model);

        Task<IEnumerable<SpeedWagonContent>> BreadCrumb(SpeedWagonContent model);

        SpeedWagonContent CreateContent(string url, IDictionary<string, object> properties);
    }
}