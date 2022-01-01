using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace EasyOC.OrchardCoreIndexModels
{

    public partial class UserIndex
    {

        public int Id { get; set; }

        public int? DocumentId { get; set; }

        public string NormalizedUserName { get; set; }

        public string NormalizedEmail { get; set; }

        public string IsEnabled { get; set; } = "1";

        public string IsLockoutEnabled { get; set; } = "0";

        public DateTime? LockoutEndUtc { get; set; }

        public int AccessFailedCount { get; set; } = 0;

        public string UserId { get; set; }

    }

}



