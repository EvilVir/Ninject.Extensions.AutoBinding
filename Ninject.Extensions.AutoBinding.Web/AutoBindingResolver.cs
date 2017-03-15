using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Syntax;
using Ninject.Web.Common;

namespace Ninject.Extensions.AutoBinding.Web
{
    public class AutoBindingResolver : Ninject.Extensions.AutoBinding.AutoBindingResolver
    {
        protected override Type InjectableAttribueType { get; } = typeof(InjectableAttribute);

        protected override IBindingNamedWithOrOnSyntax<object> DoScopeConfiguration(AutoBinding.InjectableAttribute attribute, IBindingWhenInNamedWithOrOnSyntax<object> binding)
        {
            if (attribute is InjectableAttribute webAttribute && webAttribute.Scope == InjectionScope.Request)
            {
                return binding.InRequestScope();
            }

            return base.DoScopeConfiguration(attribute, binding);
        }
    }
}
