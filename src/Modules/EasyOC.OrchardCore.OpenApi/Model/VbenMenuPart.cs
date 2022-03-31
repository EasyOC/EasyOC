using AutoMapper;
using EasyOC.OrchardCore.OpenApi.Indexs;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Model
{
    [AutoMap(typeof(VbenMenuPartIndex), ReverseMap = true)]

    public class VbenMenuPart : ContentPart
    {
        public TextField MenuName { get; set; }
        public NumericField OrderNo { get; set; }
        public TextField RoutePath { get; set; }
        public TextField Icon { get; set; }
        public TextField Component { get; set; }
        public BooleanField IsExt { get; set; }
        public BooleanField Keepalive { get; set; }
        public ContentPickerField ParentMenu { get; set; } 
        public BooleanField Show { get; set; }
        public TextField MenuType { get; set; }
        public TextField ExtentionData { get; set; }


    }
}
