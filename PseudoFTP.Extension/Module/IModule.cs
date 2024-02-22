// Copyright (C) 2018 - 2024 Tony's Studio. All rights reserved.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Tonisoft.AspExtensions.Module;

public interface IModule
{
    IServiceCollection RegisterModule(IServiceCollection services);
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}

public static class ModuleExtensions
{
    // this could also be added into the DI container
    private static readonly List<IModule> _registeredModules = new();

    public static IServiceCollection RegisterModules(this IServiceCollection services, Type baseType)
    {
        IEnumerable<IModule> modules = DiscoverModules(baseType);
        foreach (IModule module in modules)
        {
            module.RegisterModule(services);
            _registeredModules.Add(module);

            Console.WriteLine($"Module {module.GetType().Name} registered.");
        }

        return services;
    }

    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        return RegisterModules(services, typeof(IModule));
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (IModule module in _registeredModules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }


    public static IEnumerable<IModule> DiscoverModules(Type type)
    {
        return type.Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }

    private static IEnumerable<IModule> DiscoverModules()
    {
        return DiscoverModules(typeof(IModule));
    }
}