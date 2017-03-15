using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Extensions.AutoBinding.Web
{
    public enum InjectionScope
    {
        Transient = AutoBinding.InjectionScope.Transient,
        Singleton = AutoBinding.InjectionScope.Singleton,
        Thread    = AutoBinding.InjectionScope.Thread,
        Request
    }
}
