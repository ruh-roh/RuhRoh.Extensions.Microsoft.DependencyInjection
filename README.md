[![Build Status](https://gotsharp.visualstudio.com/Ruh-Roh/_apis/build/status/Ruh-Roh-MS-DI-CI?branchName=alpha)](https://gotsharp.visualstudio.com/Ruh-Roh/_build/latest?definitionId=5?branchName=master)

# Ruh Roh
## Microsoft.Extensions.DependencyInjection

When the project in which you want to add chaos is using Microsoft.Extensions.DependencyInjection as the DI container,
then this project will make it a lot easier to configure your services with Ruh Roh.

This NuGet package provides three new extension methods for the `IServiceCollection` interface:

* `AffectedService<TService> AffectSingleton<TService, TImplementation>(this IServiceCollection services)`
   Use this method to reconfigure a Singleton service.

* `AffectedService<TService> AffectScoped<TService, TImplementation>(this IServiceCollection services)`
   Use this method to reconfigure a Scoped service.

* `AffectedService<TService> AffectTransient<TService, TImplementation>(this IServiceCollection services)`
   Use this method to reconfigure a service that gets a new instance each time.


All three methods replace the originally registered service with a new one, in which you can configure chaos to occur.
The actual `TImplementation` will be reregistered to allow Ruh Roh from injecting the original service in its chaos variant.

This project uses [GitHub flow](https://guides.github.com/introduction/flow/).
