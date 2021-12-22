using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Metadata.Records;
using System;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.RDBMS.ViewModels
{
    public class RecipeModel
    {
        public string name { get; set; } = string.Empty;
        public string displayName { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string author { get; set; } = string.Empty;
        public string website { get; set; } = string.Empty;
        public string version { get; set; } = string.Empty;
        public bool issetuprecipe { get; set; } = false;
        public object[] categories { get; set; } = Array.Empty<object>();
        public object[] tags { get; set; } = Array.Empty<object>();
        public List<Step> steps { get; set; }
    }

    public class Step
    {
        public string name { get; set; }
        public List<ContentType> ContentTypes { get; set; } = new List<ContentType>();
        public List<Contentpart> ContentParts { get; set; } = new List<Contentpart>();
    }

    public class ContentType
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public JObject Settings { get; set; }
        public ContentTypePartDefinitionRecord[] ContentTypePartDefinitionRecords { get; set; }
    }

    public class Settings
    {
        public Contenttypesettings ContentTypeSettings { get; set; }
    }

    public class Contenttypesettings
    {
        public bool Creatable { get; set; }
        public bool Listable { get; set; }
        public bool Draftable { get; set; }
        public bool Versionable { get; set; }
        public bool Securable { get; set; }
    }



    public class Contentpart
    {
        public string Name { get; set; }
        public ContentpartSettings Settings { get; set; }
        public ContentPartFieldDefinitionRecord[] ContentPartFieldDefinitionRecords { get; set; }
        public string DispalyName { get; set; }
    }

    public class ContentpartSettings
    {
        public Contentpartsettings ContentPartSettings { get; set; }
    }

    public class Contentpartsettings
    {
        public bool Attachable { get; set; }
        public string Description { get; set; }
        public string DefaultPosition { get; set; }
    }



    public class Contentpartfieldsettings
    {
        public string DisplayName { get; set; }
        public string Position { get; set; }
    }



    public class Textfieldpredefinedlisteditorsettings
    {
        public Option[] Options { get; set; }
        public int Editor { get; set; }
        public string DefaultValue { get; set; }
    }

    public class Option
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}






