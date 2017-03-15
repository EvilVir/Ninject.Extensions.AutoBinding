using Ninject.Extensions.AutoBinding.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.AutoBindingTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ninject.Extensions.AutoBindingTests.Mocks.BindableImplementationC;

namespace Ninject.Extensions.AutoBinding.Web.Tests
{
    [TestClass]
    public class NinjectExtensionsWebAutobindingTests
    {
        protected IKernel container;

        [TestInitialize]
        public void Initialize()
        {
            container = new StandardKernel();
        }

        [TestMethod]
        public void RequestBindingTest()
        {
            container.AutoBinding();

            IWebService webService = container.Get<IWebService>();

            Assert.IsInstanceOfType(webService, typeof(WebImplementation));
        }

        [TestMethod]
        public void ScopeBindingPassthroughTest()
        {
            container.AutoBinding();

            WebImplementation implementationA = container.Get<WebImplementation>();
            WebImplementation implementationB = container.Get<WebImplementation>();

            Assert.AreSame(implementationA, implementationB);
        }
    }
}
