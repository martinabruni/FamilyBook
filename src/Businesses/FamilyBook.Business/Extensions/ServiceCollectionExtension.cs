using FamilyBook.Domain.Validators;
using FamilyBook.Domain.Models;
using FamilyBook.Business.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyBook.Business.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // Validators are instantiated with the instance to validate
        services.AddScoped<IValidator<Family>, FamilyValidator>();
        services.AddScoped<IValidator<Member>, MemberValidator>();
        services.AddScoped<IValidator<Album>, AlbumValidator>();
        services.AddScoped<IValidator<Photo>, PhotoValidator>();

        return services;
    }
}