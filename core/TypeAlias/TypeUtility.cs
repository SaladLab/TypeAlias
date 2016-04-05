using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace TypeAlias
{
    public static class TypeUtility
    {
        private static ConcurrentDictionary<string, Type> _typeCacheMap = new ConcurrentDictionary<string, Type>();

        // AssemblyProvider that will be used for traversing all types
        public static IAssemblyProvider AssemblyProvider { get; set; } = new AssemblyProvider();

        // Return type if typeName is assembly qualified name or full name
        public static Type GetType(string typeName)
        {
            // easy case if typeName is an assembly qualified name

            var isQualifedName = typeName.Contains(", Version=");
            if (isQualifedName)
                return Type.GetType(typeName);

            // full search with cache if typename is a full name

            Type cacheType;
            if (_typeCacheMap.TryGetValue(typeName, out cacheType))
                return cacheType;

            foreach (var assembly in AssemblyProvider.GetAssemblies())
            {
                foreach (var type in GetTypes(assembly))
                {
                    if (type.FullName == typeName)
                    {
                        _typeCacheMap.TryAdd(typeName, type);
                        return type;
                    }
                }
            }

            return null;
        }

        public static Assembly GetContainingAssembly(Type type)
        {
#if COREFX
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }

        public static IEnumerable<Type> GetTypes(Assembly assembly)
        {
#if COREFX
            foreach (var typeInfo in assembly.DefinedTypes)
            {
                if (typeInfo != null)
                    yield return typeInfo.AsType();
            }
#else
            Type[] typesInAsm;
            try
            {
                typesInAsm = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                typesInAsm = e.Types;
            }

            foreach (var type in typesInAsm)
            {
                if (type != null)
                    yield return type;
            }
#endif
        }

        public struct TypeAndAttribute<TAttribute>
        {
            public Type Type;
            public TAttribute Attribute;
        }

        public static IEnumerable<TypeAndAttribute<TAttribute>> GetTypesContainingAttribute<TAttribute>(
            Assembly assembly)
            where TAttribute : Attribute
        {
#if COREFX
            foreach (var typeInfo in assembly.DefinedTypes)
            {
                var attr = typeInfo.GetCustomAttribute<TAttribute>();
                if (attr != null)
                    yield return new TypeAndAttribute<TAttribute> { Type = typeInfo.AsType(), Attribute = attr };
            }
#else
            foreach (var type in GetTypes(assembly))
            {
                var attr = (TAttribute)Attribute.GetCustomAttribute(type, typeof(TAttribute));
                if (attr != null)
                    yield return new TypeAndAttribute<TAttribute> { Type = type, Attribute = attr };
            }
#endif
        }
    }
}
