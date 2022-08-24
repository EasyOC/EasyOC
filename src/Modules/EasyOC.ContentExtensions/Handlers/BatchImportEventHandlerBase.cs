using OrchardCore.ContentManagement.Handlers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.ContentExtensions.Handlers
{
    public class BatchImportEventHandlerBase: IBatchImportEventHandler
    {

        public virtual Task BeforeImportAsync(IEnumerable<ImportContentContext> contentItems) => Task.CompletedTask;

        public virtual Task AfterImportAsync(IEnumerable<ImportContentContext> contentItems) => Task.CompletedTask;

    }
}
