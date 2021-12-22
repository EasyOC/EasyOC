using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace EasyOC.OrchardCore.RDBMS.Models
{

    public class RDBMSMappingConfig : ContentPart
    {
        public TextField ConfigName { get; set; }
        public string ContentTypeName { get; set; }
        public SyncMappingDeriction SyncMappingDeriction { get; set; }
        public string ConnectionConfigId { get; set; }
        public string TargetTable { get; set; }
        public string MappingData { get; set; }

        public bool ReadOnly { get; set; }

        public bool EnableAutoSync { get; set; }
    }

    public enum SyncMappingDeriction
    {
        OrchardCoreToRDBMS,
        RDBMSToOrchardCore,
        TwoWay

    }
}


