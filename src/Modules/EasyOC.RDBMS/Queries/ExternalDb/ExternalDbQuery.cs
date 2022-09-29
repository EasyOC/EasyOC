using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.Queries.ExternalDb;

public class ExternalDbQuery : Query
{
    public ExternalDbQuery() : base(Consts.QueryName)
    {
    }
    public string TotalQuery { get; set; }
    public bool HasTotal { get; set; }

    public string ConnectionConfigId { get; set; }

}