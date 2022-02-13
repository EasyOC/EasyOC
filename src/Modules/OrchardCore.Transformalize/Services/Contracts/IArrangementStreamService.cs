using System.Threading.Tasks;
using Transformalize.Configuration;
using System.IO;

namespace TransformalizeModule.Services.Contracts {
   public interface IArrangementStreamService {
      Task RunAsync(Process process, StreamWriter streamWriter);
      void Run(Process process, StreamWriter streamWriter);
   }
}
