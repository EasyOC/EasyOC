using EasyOC.RDBMS.Models;
using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.Queries.ScriptQuery
{
    public class ScriptQuery : Query
    {
        public ScriptQuery() : base(Consts.QueryName)
        {
        }

        public string Scripts { get; set; }
        public bool ReturnDocuments { get; set; }
    }


}
