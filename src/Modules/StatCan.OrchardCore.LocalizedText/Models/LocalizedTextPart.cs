using OrchardCore.ContentManagement;
using StatCan.OrchardCore.LocalizedText.Models;
using System.Collections.Generic;

namespace StatCan.OrchardCore.LocalizedText.Fields
{
    public class LocalizedTextPart : ContentPart
    {
        public IList<LocalizedTextEntry> Data { get; set; }
    }
}



