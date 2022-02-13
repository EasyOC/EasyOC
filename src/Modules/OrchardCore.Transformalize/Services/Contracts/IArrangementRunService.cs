using System.Threading.Tasks;
using Transformalize.Configuration;

namespace TransformalizeModule.Services.Contracts {
   public interface IArrangementRunService {
      Task RunAsync(Process process);
      void Run(Process process);
   }
}
