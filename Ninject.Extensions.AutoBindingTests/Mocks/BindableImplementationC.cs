using Ninject.Extensions.AutoBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Extensions.AutoBindingTests.Mocks
{
    /// <summary>
    /// This one should be used only if Profile_A nor Profile_B is active
    /// </summary>
    [Injectable(Interface = typeof(IBindableService), ExcludeInProfiles = new string[]{ Profiles.PROFILE_A, Profiles.PROFILE_B }, Scope = InjectionScope.Thread)]
    public class BindableImplementationC : IBindableService, IDisposable
    {
        public class DisposeCalledException : Exception
        {

        }

        public string Value { get; } = Guid.NewGuid().ToString();

        public void Dispose()
        {
            throw new DisposeCalledException();
        }
    }
}
