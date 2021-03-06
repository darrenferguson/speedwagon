﻿using System.Collections.Generic;

namespace SpeedWagon.Web.Models.Content
{
    public class ContentEditorModel
    {
        public string ContentPath { get; set; }

        public string PropertyName { get; set; }

        public IDictionary<string, string> Values { get; set; }
    }
}