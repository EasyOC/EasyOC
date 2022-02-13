using System.Collections.Generic;
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Reader;
using Cfg.Net.Shorthand;
using TransformalizeModule.Services.Transforms;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Containers.Autofac.Modules;
using Transformalize.Contracts;
using Transformalize.Providers.File.Autofac;
using Transformalize.Transforms.Humanizer.Autofac;
using Transformalize.Transforms.Json.Autofac;
using Transformalize.Validate.Jint.Autofac;
using Transformalize.Transforms.LambdaParser.Autofac;
using Transformalize.Transforms.Aws.Autofac;

namespace TransformalizeModule.Services.Modules {

   public class ShorthandModule : Module {

      private readonly HashSet<string> _methods = new HashSet<string>();
      private readonly ShorthandRoot _shortHand = new ShorthandRoot();
      private readonly IPipelineLogger _logger;

      public ShorthandModule(
         IPipelineLogger logger
      ) {
         _logger = logger;
      }

      protected override void Load(ContainerBuilder builder) {

         builder.Register((ctx) => _logger).As<IPipelineLogger>();

         builder.Register<IReader>(c => new DefaultReader(new FileReader(), new WebReader())).As<IReader>();

         // register short-hand for t attribute
         var tm = new TransformModule(new Process { Name = "Transform Shorthand" }, _methods, _shortHand, _logger) { Plugins = false };
         // adding additional transforms here
         tm.AddTransform(new TransformHolder((c) => new UsernameTransform(), new UsernameTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new UserIdTransform(), new UserIdTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new UserEmailTransform(), new UserEmailTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new OrchardRazorTransform(), new OrchardRazorTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new OrchardFluidTransform(), new OrchardFluidTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new OrchardJintTransform(), new OrchardJintTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new ToLocalTimeTransform(), new ToLocalTimeTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new GetEncodedUrlTransform(), new GetEncodedUrlTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new GetDisplayUrlTransform(), new GetDisplayUrlTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new OrchardTimeZoneTransform(), new OrchardTimeZoneTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new FilePartTransform(), new FilePartTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new AddQueryParameterTransform(), new AddQueryParameterTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new RemoveQueryParameterTransform(), new RemoveQueryParameterTransform().GetSignatures()));
         tm.AddTransform(new TransformHolder((c) => new RemoveQueryParametersTransform(), new RemoveQueryParametersTransform().GetSignatures()));

         builder.RegisterModule(tm);

         // register short-hand for v attribute
         var vm = new ValidateModule(new Process { Name = "ValidateShorthand" }, _methods, _shortHand, _logger) { Plugins = false };
         // adding additional validators here
         builder.RegisterModule(vm);

         // register the validator short hand
         builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(ValidateModule.FieldsName).InstancePerLifetimeScope();
         builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(ValidateModule.ParametersName).InstancePerLifetimeScope();
         builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(ValidateModule.FieldsName), new[] { "fields", "calculated-fields", "calculatedfields" }, "v", "validators", "method")).Named<IDependency>(ValidateModule.FieldsName).InstancePerLifetimeScope();
         builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(ValidateModule.ParametersName), new[] { "parameters" }, "v", "validators", "method")).Named<IDependency>(ValidateModule.ParametersName).InstancePerLifetimeScope();

         // register the transform short hand
         builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(TransformModule.FieldsName).InstancePerLifetimeScope();
         builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(TransformModule.ParametersName).InstancePerLifetimeScope();
         builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(TransformModule.FieldsName), new[] { "fields", "calculated-fields", "calculatedfields" }, "t", "transforms", "method")).Named<IDependency>(TransformModule.FieldsName).InstancePerLifetimeScope();
         builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(TransformModule.ParametersName), new[] { "parameters" }, "t", "transforms", "method")).Named<IDependency>(TransformModule.ParametersName).InstancePerLifetimeScope();

         // the shorthand registrations are stored in the builder's properties for plugins to add to
         builder.Properties["ShortHand"] = _shortHand;
         builder.Properties["Methods"] = _methods;

         // register transform modules here so they can add their shorthand
         builder.RegisterModule(new JsonTransformModule());
         builder.RegisterModule(new HumanizeModule());
         builder.RegisterModule(new FileModule());
         builder.RegisterModule(new LambdaParserModule());
         builder.RegisterModule(new AwsTransformModule());

         // register validator modules here so they can register their short-hand
         builder.RegisterModule(new JintValidateModule());

         builder.Register((c, p) => _methods).Named<HashSet<string>>("Methods").InstancePerLifetimeScope();
         builder.Register((c, p) => _shortHand).As<ShorthandRoot>().InstancePerLifetimeScope();
      }
   }
}
