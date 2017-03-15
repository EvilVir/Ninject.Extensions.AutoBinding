using Ninject.Extensions.AutoBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Extensions.AutoBindingTests.Mocks
{
    [Injectable]
    [Injectable(Interface = typeof(IMaster), Profiles = new string[] { Profiles.PROFILE_A })]
    [Injectable(Interface = typeof(IMaster2), Profiles = new string[] { Profiles.PROFILE_A, Profiles.PROFILE_D })]
    public class Master : IMaster, IMaster2
    {
        [Inject]
        public IBindableService Service { get; set; }
    }
}
