using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Extensions.AutoBinding
{
    /// <summary>
    /// Mark any class with this attribute, then use <seealso cref="AutoBinding"/> extensions to automatically detect them and bind on runtime.
    /// You can use this attribute more than once on single class to achieve bindings to multiple interfaces etc.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class InjectableAttribute : Attribute
    {
        /// <summary>
        /// Type to which class will be bound. If not provided then it'll be bound to self.
        /// </summary>
        public Type Interface { get; set; }

        /// <summary>
        /// List of profile names in which interface implementation will be available.
        /// For example: you can make different implementation be choosen whenever you're in TestProfile and other in ProductionProfile etc.
        /// </summary>
        public string[] Profiles { get; set; }

        /// <summary>
        /// List of profile names in which interface implementation will NOT be available.
        /// Sometimes it's easier to not provide <seealso cref="Profiles"/> parameter but instead just exclude some implementations.
        /// </summary>
        public string[] ExcludeInProfiles { get; set; }

        /// <summary>
        /// Injection scope for this binding
        /// </summary>
        public virtual InjectionScope Scope { get; set; } = InjectionScope.Transient;

        /// <summary>
        /// If your class implements IDisposable, then it's Dispose() method will be called by Ninject when lifecycle of injectable ends.
        /// You can set this property to true so it won't be called.
        /// </summary>
        public bool IgnoreDisposable { get; set; } = false;

        public InjectableAttribute()
        {
        }

        public InjectableAttribute(Type @interface, params string[] profiles)
        {
            this.Interface = @interface;
            this.Profiles = profiles;
        }

        int GetScopeCode()
        {
            return (int)Scope;
        }
    }
}
