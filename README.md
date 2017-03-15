# Ninject.Extensions.AutoBinding
Adds ability to automatically discover injectable classes that are marked with simple attribute

## Basic usage

### Marking classes for autoinjection

Binding to self:
```csharp
[Injectable]
public class MyService : IService
{
}
```

Binding to interface:
```csharp
[Injectable(Interface = typeof(IService)]
public class MyService : IService
{
}
```

Binding to multiple interfaces:
```csharp
[Injectable(Interface = typeof(IService)]
[Injectable(Interface = typeof(IAnotherService)]
public class MyService : IService, IAnotherService
{
}
```

### Injecting

Just use standard ``[Inject]`` attribute from Ninject:

```csharp
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
IKernel container = new StandardKernel().AutoBinding();
```

## Scopes

You can controll in which scope your injectable will be bound ([check this link for more info about Ninject scopes](https://github.com/ninject/ninject/wiki/Object-Scopes)). Just use Scope parameter and provide one of ``InjectionScope`` enum values. Default scope is ``Transient``.

```csharp
[Injectable(Interface = typeof(IService), Scope: InjectionScope.Singleton]
public class MyService : IService
{
}
```

## Profiles

Sometimes you might want to use different implementation depending on execution environment conditions. For example you'll use sandboxed WebService API during development and production one on production server. With this extension you can easly configure which implementation of your classes will be injected. Just use ``Profiles`` property for that and then provide list of profiles to ``AutoBinding(params string[])`` extension method.

```csharp
[Injectable(Profiles = "DEV_MODE", Interface = typeof(IWebServiceApi))]
public class DevModeSandboxedService : IWebServiceApi
{
}

[Injectable(Profiles = "PRODUCTION_MODE", Interface = typeof(IWebServiceApi))]
public class ProductionModeService : IWebServiceApi
{
}

IKernel container = new StandardKernel().AutoBinding("DEV_MODE");
IWebServiceApi apiClient = container.Get<IWebServiceApi>(); // Will return DevModeSandboxedService implementation
```

You can also exclude classes from certain profiles. Let's change previous example a little bit:

```csharp
[Injectable(Profiles = "DEV_MODE", Interface = typeof(IWebServiceApi))]
public class DevModeSandboxedService : IWebServiceApi
{
}

[Injectable(ExcludeInProfiles = "DEV_MODE", Interface = typeof(IWebServiceApi))] // Here instead of providing explicit profile name you can exclude that injectable from DEV_MODE profile.
public class ProductionModeService : IWebServiceApi
{
}

IKernel container = new StandardKernel().AutoBinding("DEV_MODE");
IWebServiceApi apiClient = container.Get<IWebServiceApi>(); // Will still return DevModeSandboxedService implementation
```