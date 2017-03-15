using Ninject.Syntax;
using System;
using System.Linq;
using System.Reflection;

namespace Ninject.Extensions.AutoBinding
{
    public static class NinjectExtensionMethods
    {
        /// <summary>
        /// Binds classes detected in all AppDomain assemblies without profiles filtering.
        /// </summary>
        /// <param name="kernel">Ninject Kernel object</param>
        /// <returns>Passed in Ninject Kernel object for fluent configuration</returns>
        public static IKernel AutoBinding(this IKernel kernel)
        {
            return AutoBinding(kernel, seekInAssemblies: null, activeProfiles: null);
        }

        /// <summary>
        /// Binds classes detected in all AppDomain assemblies with profiles filtering.
        /// </summary>
        /// <param name="kernel">Ninject Kernel object</param>
        /// <param name="activeProfiles">List of active profiles on which bindable classes will be filtered</param>
        /// <returns>Passed in Ninject Kernel object for fluent configuration</returns>
        public static IKernel AutoBinding(this IKernel kernel, params string[] activeProfiles)
        {
            return AutoBinding(kernel, seekInAssemblies: null, activeProfiles: activeProfiles);
        }

        /// <summary>
        /// Binds classes detected in given list of assemblies without profiles filtering.
        /// </summary>
        /// <param name="kernel">Ninject Kernel object</param>
        /// <param name="seekInAssemblies">List of assemblied to seek bindable classes in</param>
        /// <returns>Passed in Ninject Kernel object for fluent configuration</returns>
        public static IKernel AutoBinding(this IKernel kernel, Assembly[] seekInAssemblies)
        {
            return AutoBinding(kernel, seekInAssemblies: seekInAssemblies, activeProfiles: null);
        }

        /// <summary>
        /// Binds classes detected in given list of assemblies with profiles filtering.
        /// </summary>
        /// <param name="kernel">Ninject Kernel object</param>
        /// <param name="seekInAssemblies">List of assemblied to seek bindable classes in</param>
        /// <param name="activeProfiles">List of active profiles on which bindable classes will be filtered</param>
        /// <returns>Passed in Ninject Kernel object for fluent configuration</returns>
        public static IKernel AutoBinding(this IKernel kernel, Assembly[] seekInAssemblies, params string[] activeProfiles)
        {
            foreach (Assembly assembly in seekInAssemblies ?? AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type targetType in assembly.GetTypes().Where(x => x.GetCustomAttributes(typeof(InjectableAttribute), true).Count() > 0))
                {
                    foreach (InjectableAttribute attribute in targetType.GetCustomAttributes(typeof(InjectableAttribute), true))
                    {
                        DoBinding(kernel, targetType, attribute, activeProfiles);
                    }
                }
            }

            return kernel;
        }

        /// <summary>
        /// Does actual binding of target type.
        /// </summary>
        /// <param name="targetType">Type that is bound</param>
        /// <param name="attribute">Attribute from that type that will be used for that binding</param>
        /// <param name="activeProfiles">List of active profiles or null if no profiles mode</param>
        private static void DoBinding(IKernel kernel, Type targetType, InjectableAttribute attribute, string[] activeProfiles)
        {
            if (activeProfiles != null)
            {
                // If none of profiles match current active profiles then we don't bind that type
                if (attribute.Profiles != null)
                {
                    bool foundMatch = false;
                    foreach (string profile in attribute.Profiles)
                    {
                        if (activeProfiles.Contains(profile))
                        {
                            foundMatch = true;
                            break;
                        }
                    }

                    if (!foundMatch)
                    {
                        return;
                    }
                }

                // If any of excluded profiles match one of current active profiles then we don't bind that type
                if (attribute.ExcludeInProfiles != null)
                {
                    foreach (string profile in attribute.ExcludeInProfiles)
                    {
                        if (activeProfiles.Contains(profile))
                        {
                            return;
                        }
                    }
                }
            }

            IBindingWhenInNamedWithOrOnSyntax<object> binding1 = kernel.Bind(attribute.Interface ?? targetType).To(targetType);
            IBindingNamedWithOrOnSyntax<object> binding2 = null;

            switch (attribute.Scope)
            {
                default: case InjectionScope.Transient: binding2 = binding1.InTransientScope(); break;
                case InjectionScope.Singleton: binding2 = binding1.InSingletonScope(); break;
                case InjectionScope.Thread: binding2 = binding1.InThreadScope(); break;
            }

            if (!attribute.IgnoreDisposable && typeof(IDisposable).IsAssignableFrom(targetType))
            {
                binding2.OnDeactivation(x => ((IDisposable)x).Dispose());
            }
        }
    }
}
