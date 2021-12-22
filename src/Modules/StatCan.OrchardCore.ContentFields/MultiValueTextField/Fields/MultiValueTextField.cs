using OrchardCore.ContentManagement;
using System;

namespace StatCan.OrchardCore.ContentFields.MultiValueTextField.Fields
{
    public class MultiValueTextField : ContentField
    {
        public string[] Values { get; set; } = Array.Empty<string>();
    }
}



