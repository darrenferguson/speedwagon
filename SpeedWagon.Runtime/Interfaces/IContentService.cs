using SpeedWagon.Models;
using System;
using System.Collections.Generic;


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

        void RefreshUrls();
        IEnumerable<string> GetUrlList();

        SpeedWagonContent GetContent(string url);
        SpeedWagonContent Home(SpeedWagonContent model);

        IEnumerable<SpeedWagonContent> TopNavigation(SpeedWagonContent model);
        IEnumerable<SpeedWagonContent> Children(SpeedWagonContent model);

        IEnumerable<SpeedWagonContent> Descendants(SpeedWagonContent model);
        IEnumerable<SpeedWagonContent> Descendants(SpeedWagonContent model, IDictionary<string, string> filter);

        SpeedWagonContent Parent(SpeedWagonContent model);

        IEnumerable<SpeedWagonContent> BreadCrumb(SpeedWagonContent model);

        SpeedWagonContent CreateContent(string url, IDictionary<string, object> properties);
    }
}