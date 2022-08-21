using OrchardCore.ContentManagement.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.ContentExtentions.Handlers
{
    public class BatchImportEventHandlerBase: IBatchImportEventHandler
    {

        public virtual Task BeforeImportAsync(IEnumerable<ImportContentContext> contentItems) => Task.CompletedTask;

        public virtual Task AfterImportAsync(IEnumerable<ImportContentContext> contentItems) => Task.CompletedTask;

    }
}
