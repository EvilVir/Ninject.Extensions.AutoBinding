using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ninject.Extensions.AutoBinding.Web
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
            return new AutoBindingResolver().AutoBinding(kernel);
        }

        /// <summary>
        /// Binds classes detected in all AppDomain assemblies with profiles filtering.
        /// </summary>
        /// <param name="kernel">Ninject Kernel object</param>
        /// <param name="activeProfiles">List of active profiles on which bindable classes will be filtered</param>
        /// <returns>Passed in Ninject Kernel object for fluent configuration</returns>
        public static IKernel AutoBinding(this IKernel kernel, params string[] activeProfiles)
        {
            return new AutoBindingResolver().AutoBinding(kernel, activeProfiles);
        }

        /// <summary>
        /// Binds classes detected in given list of assemblies without profiles filtering.
        /// </summary>
        /// <param name="kernel">Ninject Kernel object</param>
        /// <param name="seekInAssemblies">List of assemblied to seek bindable classes in</param>
        /// <returns>Passed in Ninject Kernel object for fluent configuration</returns>
        public static IKernel AutoBinding(this IKernel kernel, Assembly[] seekInAssemblies)
        {
            return new AutoBindingResolver().AutoBinding(kernel, seekInAssemblies);
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
            return new AutoBindingResolver().AutoBinding(kernel, seekInAssemblies, activeProfiles);
        }
    }
}
