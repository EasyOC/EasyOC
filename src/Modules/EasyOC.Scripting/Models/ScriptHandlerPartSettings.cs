
using System.Collections.Generic;

namespace EasyOC.Scripting.Models
{
    public class ScriptHandlerPartSettings
    {       /// <summary>
            /// ½ûÓÃÈ«²¿
            /// </summary>
        public bool Disabled { get; set; }

        public List<ScriptHandlerSettingItem> ScriptHandlerSettingItems { get; set; } = new List<ScriptHandlerSettingItem>();

    }

    public class ScriptHandlerSettingItem
    {
        public bool Disabled { get; set; }
        public string Script { get; set; }
        public string EventName { get; set; }
        public int Order { get; set; }
    }
     
}
