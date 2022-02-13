#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Autofac;
using System.Linq;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using TransformalizeModule.Services.Writers;

namespace TransformalizeModule.Services.Modules {

   /// <inheritdoc />
   /// <summary>
   /// Registers all the built-in transforms
   /// </summary>
   public class OrchardLogModule : Module {

      private const string LogProvider = "log";
      private readonly Process _process;

      /// <summary>
      /// Register internal actions, connections, readers, and writers
      /// </summary>
      /// <param name="process">the arrangement</param>
      public OrchardLogModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null)
            return;

         foreach (var action in _process.Templates.Where(t => t.Enabled).SelectMany(t => t.Actions).Where(a => a.Type == "log" && a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            builder.Register(ctx => SwitchAction(ctx, _process, action)).Named<IAction>(action.Key);
         }
         foreach (var action in _process.Actions.Where(a => a.Type == "log" && a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            builder.Register(ctx => SwitchAction(ctx, _process, action)).Named<IAction>(action.Key);
         }

         if (_process.Connections.All(c => c.Provider != LogProvider)) {
            return;
         }

         // add null schema reader for each internal connection
         foreach (var connection in _process.Connections.Where(c => c.Provider == LogProvider)) {
            builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
         }

         // PROCESS AND ENTITY OUTPUT
         // if output is internal, setup internal output controllers for the process and each entity
         if (_process.GetOutputConnection().Provider == LogProvider) {

            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in _process.Entities) {

               builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);
               builder.Register<IOutputProvider>(ctx => new NullOutputProvider()).Named<IOutputProvider>(entity.Key);

               // WRITER
               builder.Register<IWrite>(ctx => new LogWriter(ctx.ResolveNamed<OutputContext>(entity.Key))).Named<IWrite>(entity.Key);
            }
         }

         // ENTITY INPUT
         // setup internal input readers for each entity if necessary
         foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Input).Provider == LogProvider)) {

            builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

            // READER
            builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               return new NullReader(input);
            }).Named<IRead>(entity.Key);
         }
      }

      private static IAction SwitchAction(IComponentContext ctx, Process process, Action action) {

         var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), process);

         switch (action.Type) {
            case "log":
               return new LogAction(context, action);
            default:
               context.Error("{0} action is not registered.", action.Type);
               return new NullAction();
         }
      }

   }
}
