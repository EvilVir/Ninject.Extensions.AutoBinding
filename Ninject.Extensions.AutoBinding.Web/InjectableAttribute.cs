using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Extensions.AutoBinding.Web
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class InjectableAttribute : Ninject.Extensions.AutoBinding.InjectableAttribute
    {
        private InjectionScope _scope = InjectionScope.Transient;
        public new InjectionScope Scope
        {
            get
            {
                return _scope;
            }

            set
            {
                _scope = value;
                base.Scope = (AutoBinding.InjectionScope)value;
            }
        }
    }
}
