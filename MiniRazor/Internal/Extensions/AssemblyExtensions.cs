﻿using System.Collections.Generic;
using System.Reflection;

namespace MiniRazor.Internal.Extensions
{
    internal static class AssemblyExtensions
    {
        public static Assembly Load(this AssemblyName assemblyName) =>
            Assembly.Load(assemblyName);

        public static Assembly? TryLoad(this AssemblyName assemblyName)
        {
            try
            {
                return assemblyName.Load();
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<AssemblyName> GetTransitiveAssemblies(this Assembly assembly)
        {
            var assemblyNames = new HashSet<AssemblyName>(AssemblyNameEqualityComparer.Instance);
            assembly.PopulateTransitiveAssemblies(assemblyNames);

            return assemblyNames;
        }

        private static void PopulateTransitiveAssemblies(this Assembly assembly, ISet<AssemblyName> assemblyNames)
        {
            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                // Already exists
                if (!assemblyNames.Add(referencedAssemblyName))
                    continue;

                var referencedAssembly = referencedAssemblyName.TryLoad();
                if (referencedAssembly != null)
                    referencedAssembly.PopulateTransitiveAssemblies(assemblyNames);
            }
        }
    }
}