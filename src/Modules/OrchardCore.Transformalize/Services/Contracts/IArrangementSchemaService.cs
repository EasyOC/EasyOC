using System.Threading.Tasks;
using Transformalize.Configuration;

namespace TransformalizeModule.Services.Contracts {
   public interface IArrangementSchemaService {
      Task<Process> GetSchemaAsync(Process process);
   }
}
