using System;
using System.Collections.Generic;
using System.Reflection;
using EasyOC;
using Natasha.CSharp;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.CSharpScript.Services
{
    public class CSharpScriptProvider : ICSharpScriptProvider
    {
        private static bool _initState = false;
        private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private AssemblyCSharpBuilder _builder;

        public virtual async Task<AssemblyCSharpBuilder> GetAssemblyCSharpBuilderAsync(
            bool useGlobalSharedBuilder = true)
        {
            // if (!_initState)
            // {
            //     _initState = true;
            //     await NatashaInitializer.InitializeAndPreheating();
            // }

            if (useGlobalSharedBuilder)
            {
                return _builder ??= new AssemblyCSharpBuilder();
            }
            else
            {
                return new AssemblyCSharpBuilder();
            }
        }

        public virtual Type GetType(string fullTypeName)
        {
            return _types.ContainsKey(fullTypeName) ? _types[fullTypeName] : null;
        }

        public virtual async Task<Type> GetOrCreateAsync(string fullName, string cSharpScripts,HashSet<string>? usings = default)
        {
            if (_types.ContainsKey(fullName))
            {
                return _types[fullName];
            }
            else
            {
                var builder = await GetAssemblyCSharpBuilderAsync();
                builder.Add(cSharpScripts,usings);
                var asm = builder.GetAssembly();
                var type = asm.GetType(fullName);
                _types.Add(fullName, type);
                return type;
            }
        }

        public virtual async Task<Type> CreateTypeAsync(string fullName, string cSharpScripts,
            HashSet<string>? usings = default)
        {
            try
            {


                var builder = await GetAssemblyCSharpBuilderAsync();
                builder.Add(cSharpScripts, usings);
                var asm = builder.GetAssembly();
                var type = asm.GetType(fullName);

                if (_types.ContainsKey(fullName))
                {
                    _types[fullName] = type;
                }
                else
                {
                    _types.Add(fullName, type);
                }

                return type;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
