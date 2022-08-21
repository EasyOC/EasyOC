using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class OpenId_OpenIdAppByRoleNameIndex_OpenId_Document
    {

        [JsonProperty, Column(DbType = "INT")]
        public int OpenIdAppByRoleNameIndexId { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int DocumentId { get; set; }

    }

}



