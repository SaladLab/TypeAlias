using System;
using System.Collections.Generic;
using System.Reflection;

namespace TypeAlias
{
    public interface IAssemblyProvider
    {
        IEnumerable<Assembly> GetAssemblies();
    }

    public class AssemblyProvider : IAssemblyProvider
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
#if COREFX
            // Stub implementation
            return new Assembly[] { typeof(IAssemblyProvider).GetTypeInfo().Assembly };
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }
    }

    public class FixedAssemblyProvider : IAssemblyProvider
    {
        private Assembly[] _assemblies;

        public FixedAssemblyProvider(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            return _assemblies;
        }
    }
}
