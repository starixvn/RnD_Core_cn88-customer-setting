namespace CN88.CustomerSetting.Client.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Compilation;

    internal static class TypeDetection
    {
        public static IEnumerable<Type> FilterTypesInAssemblies(Predicate<Type> predicate)
        {
            IEnumerable<Type> types = Type.EmptyTypes;
            IEnumerable<Type> matchingTypes = Type.EmptyTypes;
            ICollection assemblies = BuildManager.GetReferencedAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] typesInAsm;

                try
                {
                    typesInAsm = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    typesInAsm = ex.Types;
                }

                types = types.Concat(typesInAsm);
            }

            matchingTypes = types.Where(type => TypeIsPublicClass(type) && predicate(type));

            return matchingTypes;
        }

        private static bool TypeIsPublicClass(Type type)
        {
            return type != null && type.IsPublic && type.IsClass && !type.IsAbstract;
        }
    }
}