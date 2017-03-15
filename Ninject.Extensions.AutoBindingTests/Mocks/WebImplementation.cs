using Ninject.Extensions.AutoBinding.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Extensions.AutoBindingTests.Mocks
{
    [Injectable(Scope = InjectionScope.Singleton)]
    [Injectable(Interface = typeof(IWebService), Scope = InjectionScope.Request)]
    public class WebImplementation : IWebService
    {
    }
}
