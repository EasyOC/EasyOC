using Autofac;
using TransformalizeModule.Services.Contracts;
using StackExchange.Profiling;
using System.Linq;
using System.Threading.Tasks;
using Transformalize.Configuration;
using Transformalize.Contracts;
using IContainer = TransformalizeModule.Services.Contracts.IContainer;
using System.IO;

namespace TransformalizeModule.Services {

   public class ArrangementStreamService : IArrangementStreamService {

      private readonly IContainer _container;
      private readonly CombinedLogger<ArrangementStreamService> _logger;

      public ArrangementStreamService(
         IContainer container,
         CombinedLogger<ArrangementStreamService> logger
      ) {
         _container = container;
         _logger = logger;
      }

      public async Task RunAsync(Process process, StreamWriter streamWriter) {

         IProcessController controller;

         using (MiniProfiler.Current.Step("Run.Prepare")) {
            controller = _container.CreateScope(process, _logger, streamWriter).Resolve<IProcessController>();
         }

         using (MiniProfiler.Current.Step("Run.Execute")) {
            await controller.ExecuteAsync();
         }

         if (process.Errors().Any() || _logger.Log.Any(l => l.LogLevel == LogLevel.Error)) {
            process.Status = 500;
            process.Message = "Error";
         } else {
            process.Status = 200;
            process.Message = "Ok";
         }

         return;

      }

      public void Run(Process process, StreamWriter streamWriter) {

         IProcessController controller;

         using (MiniProfiler.Current.Step("Run.Prepare")) {
            controller = _container.CreateScope(process, _logger, streamWriter).Resolve<IProcessController>();
         }

         using (MiniProfiler.Current.Step("Run.Execute")) {
            controller.Execute();
         }

         if (process.Errors().Any() || _logger.Log.Any(l => l.LogLevel == LogLevel.Error)) {
            process.Status = 500;
            process.Message = "Error";
         } else {
            process.Status = 200;
            process.Message = "Ok";
         }

         return;

      }
   }
}
