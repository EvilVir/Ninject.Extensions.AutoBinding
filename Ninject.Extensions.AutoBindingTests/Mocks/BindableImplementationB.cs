using Ninject.Extensions.AutoBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Extensions.AutoBindingTests.Mocks
{

    [Injectable(Interface = typeof(IBindableService), Profiles = new string[]{ Profiles.PROFILE_B }, Scope = InjectionScope.Singleton)]
    public class BindableImplementationB : IBindableService
    {
        public string Value { get; } = Guid.NewGuid().ToString();
    }
}
