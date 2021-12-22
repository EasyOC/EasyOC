using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class UserByRoleNameIndex_Document
    {

        [JsonProperty, Column(DbType = "INT")]
        public int UserByRoleNameIndexId { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int DocumentId { get; set; }

    }

}



