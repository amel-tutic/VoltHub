using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VoltHub.Application.Common.Behaviors;

namespace VoltHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);          // finds every handler in this project
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));  // runs validation before each handler
        });
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        return services;
    }
}
