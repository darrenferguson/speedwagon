﻿using SpeedWagon.Models;
using SpeedWagon.Web.Models.ContentType;
using System.Collections.Generic;

namespace SpeedWagon.Web.Interfaces
{
    public interface IContentTypeService
    {
        IEnumerable<SpeedWagonContent> List();

        SpeedWagonContent Get(string name);

        ContentTypeEditor[] GetEditors(SpeedWagonContent contentType);

        void AddEditor(SpeedWagonContent contentType, ContentTypeEditor editor);

        void Add(string name, string user, bool root, IEnumerable<string> children);

        void Delete(string name);

        void Save(SpeedWagonContent contentType, string user);

        void MoveEditorUp(SpeedWagonContent contentType, string editor);

        void MoveEditorDown(SpeedWagonContent contentType, string editor);

        void DeleteEditor(SpeedWagonContent contentType, string editor);
    }
}
