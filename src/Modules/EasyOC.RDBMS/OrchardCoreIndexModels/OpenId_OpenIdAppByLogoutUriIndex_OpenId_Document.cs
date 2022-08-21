using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class OpenId_OpenIdAppByLogoutUriIndex_OpenId_Document
    {

        [JsonProperty, Column(DbType = "INT")]
        public int OpenIdAppByLogoutUriIndexId { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int DocumentId { get; set; }

    }

}



