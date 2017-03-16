# Ninject.Extensions.AutoBinding [![NuGet Version](http://img.shields.io/nuget/v/Ninject.Extensions.AutoBinding.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extensions.AutoBinding)
Adds ability to automatically discover injectable classes that are marked with simple attribute

## Basic usage

### Marking classes for autoinjection

Binding to self:
```csharp
using Ninject.Extensions.AutoBinding;

[Injectable]
public class MyService : IService
{
}
```

Binding to interface:
```csharp
using Ninject.Extensions.AutoBinding;

[Injectable(Interface = typeof(IService)]
public class MyService : IService
{
}
```

Binding to multiple interfaces:
```csharp
using Ninject.Extensions.AutoBinding;

[Injectable(Interface = typeof(IService)]
[Injectable(Interface = typeof(IAnotherService)]
public class MyService : IService, IAnotherService
{
}
```

### Injecting

Just use standard ``[Inject]`` attribute from Ninject:

```csharp
using Ninject;

public class MyController
{
	[Inject] // Property injection
	public IService InjectedService { get; set; }

	[Inject] // Constructor injection
	public MyController(IAnotherService anotherInjectedService)
	{
	}
}
```

### Making this all running

When configuring your container (``IKernel``) just call ``AutoBinding()`` extension method like so:

```csharp
using Ninject.Extensions.AutoBinding;

IKernel container = new StandardKernel().AutoBinding();
```

## Scopes

You can controll in which scope your injectable will be bound ([check this link for more info about Ninject scopes](https://github.com/ninject/ninject/wiki/Object-Scopes)). Just use Scope parameter and provide one of ``InjectionScope`` enum values. Default scope is ``Transient``.

```csharp
using Ninject.Extensions.AutoBinding;

[Injectable(Interface = typeof(IService), Scope = InjectionScope.Singleton]
public class MyService : IService
{
}
```

## Profiles

Sometimes you might want to use different implementation depending on execution environment conditions. For example you'll use sandboxed WebService API during development and production one on production server. With this extension you can easly configure which implementation of your classes will be injected. Just use ``Profiles`` property for that and then provide list of profiles to ``AutoBinding(params string[])`` extension method.

```csharp
using Ninject.Extensions.AutoBinding;

[Injectable(Profiles =  new string[]{ "DEV_MODE" }, Interface = typeof(IWebServiceApi))]
public class DevModeSandboxedService : IWebServiceApi
{
}

[Injectable(Profiles =  new string[]{ "PRODUCTION_MODE" }, Interface = typeof(IWebServiceApi))]
public class ProductionModeService : IWebServiceApi
{
}

IKernel container = new StandardKernel().AutoBinding("DEV_MODE");
IWebServiceApi apiClient = container.Get<IWebServiceApi>(); // Will return DevModeSandboxedService implementation
```

You can also exclude classes from certain profiles. Let's change previous example a little bit:

```csharp
using Ninject.Extensions.AutoBinding;

[Injectable(Profiles =  new string[]{ "DEV_MODE" }, Interface = typeof(IWebServiceApi))]
public class DevModeSandboxedService : IWebServiceApi
{
}

[Injectable(ExcludeInProfiles =  new string[]{ "DEV_MODE" }, Interface = typeof(IWebServiceApi))] // Here instead of providing explicit profile name you can exclude that injectable from DEV_MODE profile.
public class ProductionModeService : IWebServiceApi
{
}

IKernel container = new StandardKernel().AutoBinding("DEV_MODE");
IWebServiceApi apiClient = container.Get<IWebServiceApi>(); // Will still return DevModeSandboxedService implementation
```

---


# Ninject.Extensions.AutoBinding.Web [![NuGet Version](http://img.shields.io/nuget/v/Ninject.Extensions.AutoBinding.Web.svg?style=flat)](https://www.nuget.org/packages/Ninject.Extensions.AutoBinding.Web)
If you're working on Web Application project use this nuget, instead of main one, to be able to use [Ninject.Web.Common](https://github.com/ninject/Ninject.Web.Common)'s RequestScope when configuring your injectables.

```csharp
using Ninject.Extensions.AutoBinding.Web;

[Injectable(Interface = typeof(IService), Scope = InjectionScope.Request]
public class MyService : IService
{
}
```

Everything else remains the same as documented above as Web project extends base one - just remember to use ``using Ninject.Extensions.AutoBinding.Web;`` instead of ``using Ninject.Extensions.AutoBinding;`` :)