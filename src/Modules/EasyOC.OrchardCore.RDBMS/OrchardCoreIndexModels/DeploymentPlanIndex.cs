﻿using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class DeploymentPlanIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Name { get; set; }

    }

}



