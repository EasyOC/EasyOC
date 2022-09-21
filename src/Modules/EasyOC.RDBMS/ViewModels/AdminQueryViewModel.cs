using EasyOC.RDBMS.Queries.ScriptQuery.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.RDBMS.ViewModels
{
    public class AdminQueryViewModel
    {
        public string DecodedQuery { get; set; }
        public bool ReturnDocuments { get; set; }
        public string Parameters { get; set; } = "";
        [BindNever]
        public string ConsoleText { get; set; } = "";
        [BindNever]
        public TimeSpan Elapsed { get; set; } = TimeSpan.Zero;

        [BindNever]
        public object Result { get; set; }


    }
}