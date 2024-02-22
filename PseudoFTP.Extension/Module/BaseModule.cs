// Copyright (C) 2018 - 2024 Tony's Studio. All rights reserved.

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Tonisoft.AspExtensions.Module;

public class BaseModule : IModule
{
    public virtual IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        return endpoints;
    }


    public virtual IServiceCollection RegisterModule(IServiceCollection services)
    {
        return services;
    }
}