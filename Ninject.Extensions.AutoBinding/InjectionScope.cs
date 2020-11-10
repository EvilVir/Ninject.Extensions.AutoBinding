using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Extensions.AutoBinding
{
    public enum InjectionScope
    {
        Transient,
        Singleton,
        Thread,
        Call,
        Named,
        Parent
    }
}
