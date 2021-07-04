using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.AutoBinding;
using Ninject.Extensions.AutoBindingTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ninject.Extensions.AutoBindingTests.Mocks.BindableImplementationC;

namespace Ninject.Extensions.AutoBinding.Tests
{
    [TestClass]
    public class NinjectExtensionsAutobindingTests
    {
        protected IKernel container;

        [TestInitialize]
        public void Initialize()
        {
            container = new StandardKernel();
        }

        [TestMethod]
        public void ProfileABindingTest()
        {
            container.AutoBinding(Profiles.PROFILE_A);

            Master master = container.Get<Master>();

            Assert.IsNotNull(master);
            Assert.IsInstanceOfType(master.Service, typeof(BindableImplementationA));
        }

        [TestMethod]
        public void ProfileBBinding_Test()
        {
            container.AutoBinding(Profiles.PROFILE_B);

            Master master = container.Get<Master>();

            Assert.IsNotNull(master);
            Assert.IsInstanceOfType(master.Service, typeof(BindableImplementationB));
        }

        [TestMethod]
        public void ProfileCBinding_Test()
        {
            container.AutoBinding(Profiles.PROFILE_C);

            Master master = container.Get<Master>();

            Assert.IsNotNull(master);
            Assert.IsInstanceOfType(master.Service, typeof(BindableImplementationC));
        }

        [TestMethod]
        public void TransientScopeTest()
        {
            container.AutoBinding(Profiles.PROFILE_A);

            IBindableService instance1 = container.Get<IBindableService>();
            IBindableService instance2 = container.Get<IBindableService>();

            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void SingletonScopeTest()
        {
            container.AutoBinding(Profiles.PROFILE_B);

            IBindableService instance1 = container.Get<IBindableService>();
            IBindableService instance2 = container.Get<IBindableService>();
            BindableImplementationB instance3 = container.Get<BindableImplementationB>();
            IBindableServiceB instance4 = container.Get<IBindableServiceB>();

            Assert.AreSame(instance1, instance2);
            Assert.AreSame(instance1, instance3);
            Assert.AreSame(instance1, instance4);
        }

        [TestMethod]
        public async Task ThreadScopeTest()
        {
            container.AutoBinding(Profiles.PROFILE_C);

            IBindableService instance11 = null;
            IBindableService instance12 = null;

            IBindableService instance21 = null;
            IBindableService instance22 = null;

            Task t1 = Task.Run(() =>
            {
                instance11 = container.Get<IBindableService>();
                instance12 = container.Get<IBindableService>();
            });

            Task t2 = Task.Run(() =>
            {
                instance21 = container.Get<IBindableService>();
                instance22 = container.Get<IBindableService>();
            });

            await Task.WhenAll(t1, t2);

            Assert.IsNotNull(instance11);
            Assert.IsNotNull(instance12);
            Assert.IsNotNull(instance21);
            Assert.IsNotNull(instance22);
            Assert.AreSame(instance11, instance12);
            Assert.AreSame(instance21, instance22);
            Assert.AreNotSame(instance11, instance21);
            Assert.AreNotSame(instance12, instance22);
        }

        [TestMethod]
        [ExpectedException(typeof(DisposeCalledException))]
        public void AutoDisposeCallTest()
        {
            container.AutoBinding(Profiles.PROFILE_C);

            IBindableService instance = container.Get<IBindableService>();

            container.Dispose();
        }

        [TestMethod]
        public void MultipleBindingTest()
        {
            container.AutoBinding(Profiles.PROFILE_A);

            Master selfBindingInstance = container.Get<Master>();
            IMaster interface1BindingInstance = container.Get<IMaster>();
            IMaster2 interface2BindingInstance = container.Get<IMaster2>();

            Assert.IsInstanceOfType(selfBindingInstance, typeof(Master));
            Assert.IsInstanceOfType(interface1BindingInstance, typeof(Master));
            Assert.IsInstanceOfType(interface2BindingInstance, typeof(Master));
        }

        [TestMethod]
        public void MultipleConditionalBindingTest()
        {
            container.AutoBinding(Profiles.PROFILE_D);

            Assert.IsTrue(container.CanResolve<Master>());
            Assert.IsTrue(container.CanResolve<IMaster2>());
            Assert.IsFalse(container.CanResolve<IMaster>());
        }
    }
}