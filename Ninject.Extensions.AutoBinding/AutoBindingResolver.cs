using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ninject.Extensions.NamedScope;

namespace Ninject.Extensions.AutoBinding
{
    public class AutoBindingResolver
    {
        protected virtual Type InjectableAttribueType { get; } = typeof(InjectableAttribute);

        /// <summary>
        /// Binds classes detected in all AppDomain assemblies without profiles filtering.
        /// </summary>
        /// <param name="kernel">Ninject Kernel object</param>
        /// <returns>Passed in Ninject Kernel object for fluent configuration</returns>
        public IKernel AutoBinding(IKernel kernel)
        {
            return AutoBinding(kernel, seekInAssemblies: null, activeProfiles: null);
        }

        /// <summary>
        /// Binds classes detected in all AppDomain assemblies with profiles filtering.
        /// </summary>
        /// <param name="kernel">Ninject Kernel object</param>
        /// <param name="activeProfiles">List of active profiles on which bindable classes will be filtered</param>
        /// <returns>Passed in Ninject Kernel object for fluent configuration</returns>
        public IKernel AutoBinding(IKernel kernel, params string[] activeProfiles)
        {
            return AutoBinding(kernel, seekInAssemblies: null, activeProfiles: activeProfiles);
        }

        /// <summary>
        /// Binds classes detected in given list of assemblies without profiles filtering.
        /// </summary>
        /// <param name="kernel">Ninject Kernel object</param>
        /// <param name="seekInAssemblies">List of assemblied to seek bindable classes in</param>
        /// <returns>Passed in Ninject Kernel object for fluent configuration</returns>
        public IKernel AutoBinding(IKernel kernel, Assembly[] seekInAssemblies)
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
        public IKernel AutoBinding(IKernel kernel, Assembly[] seekInAssemblies, params string[] activeProfiles)
        {
            foreach (Assembly assembly in seekInAssemblies ?? AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type targetType in assembly.GetTypes().Where(x => x.GetCustomAttributes(InjectableAttribueType, true).Count() > 0))
                {
                    foreach (InjectableAttribute attribute in targetType.GetCustomAttributes(InjectableAttribueType, true))
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
        protected virtual void DoBinding(IKernel kernel, Type targetType, InjectableAttribute attribute, string[] activeProfiles)
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
            IBindingNamedWithOrOnSyntax<object> binding2 = DoScopeConfiguration(attribute, binding1);

            if (!attribute.IgnoreDisposable && typeof(IDisposable).IsAssignableFrom(targetType))
            {
                binding2.OnDeactivation(x => ((IDisposable)x).Dispose());
            }
        }

        protected virtual IBindingNamedWithOrOnSyntax<object> DoScopeConfiguration(InjectableAttribute attribute, IBindingWhenInNamedWithOrOnSyntax<object> binding)
        {
            switch (attribute.Scope)
            {
                default: case InjectionScope.Transient: return binding.InTransientScope();
                case InjectionScope.Singleton: return binding.InSingletonScope();
                case InjectionScope.Thread: return binding.InThreadScope();
                case InjectionScope.Call: return binding.InCallScope();
                case InjectionScope.Named: return binding.InNamedScope(attribute.ScopeName);
                case InjectionScope.Parent: return binding.InParentScope();
            }
        }
    }
}
