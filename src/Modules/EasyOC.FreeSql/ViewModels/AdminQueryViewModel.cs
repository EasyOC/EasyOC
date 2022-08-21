using Lucene.Net.Documents;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC.FreeSql.ViewModels
{
    public class AdminQueryViewModel
    {
        public string DecodedQuery { get; set; }

        public string Parameters { get; set; }

        [BindNever] public int Count { get; set; }
        [BindNever] public string RawSql { get; set; }


        [BindNever] public TimeSpan Elapsed { get; set; } = TimeSpan.Zero;
        [BindNever] public string FactoryName { get; set; }
        [BindNever] public IEnumerable<dynamic> Documents { get; set; } = Enumerable.Empty<dynamic>();
    }
}
