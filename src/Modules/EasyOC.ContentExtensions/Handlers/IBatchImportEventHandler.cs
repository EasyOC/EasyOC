using OrchardCore.ContentManagement.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.ContentExtensions.Handlers
{
    public interface IBatchImportEventHandler
    {
        Task BeforeImportAsync(IEnumerable<ImportContentContext> contentItems);

        Task AfterImportAsync(IEnumerable<ImportContentContext> contentItems);
    }
}
