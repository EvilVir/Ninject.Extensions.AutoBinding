using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Ninject.Extensions.NamedScope;

namespace Ninject.Extensions.AutoBinding
{
    public class AutoBindingResolver
    {
        protected virtual Type InjectableAttribueType { get; } = typeof(InjectableAttribute);
        private static readonly Dictionary<string, Dictionary<Type, InjectableAttribute[]>> TypeCache = new Dictionary<string, Dictionary<Type, InjectableAttribute[]>>();
        
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
          var inAssemblies = (seekInAssemblies ?? AppDomain.CurrentDomain.GetAssemblies());
          var cacheKey = string.Join("|",
              inAssemblies.Select(a => a.FullName).OrderBy(a => a)
              .ToArray());
            if (!TypeCache.TryGetValue(
                cacheKey, out var cacheResult))
            {
                cacheResult = new Dictionary<Type, InjectableAttribute[]>();
                inAssemblies
                    .SelectMany(assembly => assembly.GetTypes().Where(x => x.GetCustomAttributes(InjectableAttribueType, true).Length > 0))
                    .Select(type => new KeyValuePair<Type, InjectableAttribute[]>(type, type.GetCustomAttributes(InjectableAttribueType, true).OfType<InjectableAttribute>().ToArray()))
                    .ToList()
                    .ForEach(t => cacheResult.Add(t.Key, t.Value));
                TypeCache[cacheKey] = cacheResult;
            }

            foreach (var keyValuePair in cacheResult)
            {
                foreach (var injectableAttribute in keyValuePair.Value)
                {
                    DoBinding(kernel, keyValuePair.Key, injectableAttribute, activeProfiles);
                }
            }

            return kernel;
        }

        /// <summary>
        /// Does actual binding of target type.
        /// </summary>
        /// <param name="kernel">Kernel for the binding</param>
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

            var binding0 = !kernel.GetBindings(targetType).Any() ? kernel.Bind(targetType).To(targetType) : null;
            if (binding0 != null)
            {
                var binding1 = DoScopeConfiguration(attribute, binding0);
                if (!attribute.IgnoreDisposable && typeof(IDisposable).IsAssignableFrom(targetType))
                {
                    binding1.OnDeactivation(x => ((IDisposable)x).Dispose());
                }
            }

            if (attribute.Interface == null) return;
            kernel.Bind(attribute.Interface).ToMethod(ctx => ctx.Kernel.Get(targetType));
            
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
