using EasyOC.RDBMS.Workflows.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyOC.RDBMS.Workflows.ViewModels
{
    public class SQLTaskViewModel
    {
        [Required]
        public string SQLCommandText { get; set; }
        [Required]
        public SQLResultType ExcuteMode { get; set; }
        public string PropertyName { get; set; }
        public bool UseShellDbConnection { get; set; } = true;
        public string ConnectionConfigId { get; set; }
        public IEnumerable<SelectListItem> AllConnections { get; set; }
    }
}


