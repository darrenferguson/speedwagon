﻿using SpeedWagon.Interfaces;
using SpeedWagon.Models;

namespace SpeedWagon.Web.Interfaces
{
    public interface ISpeedWagonAdminContext
    {

        IContentService ContentService { get; }

        string Install(string user);

        SpeedWagonContent GetContent(string path);

        void AddEditor(string name, string user);

        void AddContentType(string name, string user);

        void SaveContentType(SpeedWagonContent contentType, string user);
    }
}
