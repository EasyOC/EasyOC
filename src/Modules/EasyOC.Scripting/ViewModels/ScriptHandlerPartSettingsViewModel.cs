using EasyOC.RDBMS.ViewModels;
using EasyOC.Scripting.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.Title.Models;
using System.Collections.Generic;

namespace StatCan.OrchardCore.ContentFields.MultiValueTextField.ViewModels
{
    public class ScriptHandlerPartSettingsViewModel
    {
        [BindNever]
        public bool Disabled { get; set; }
        [BindNever]
        public List<ScriptHandlerSettingItem> ScriptHandlerSettingItems { get; set; } = new List<ScriptHandlerSettingItem>();

    }
 
}
