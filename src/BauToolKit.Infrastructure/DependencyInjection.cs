#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Infrastructure.Persistence;
using BauToolKit.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace BauToolKit.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBauToolKitInfrastructure(this IServiceCollection services, string? storageRootPath = null)
    {
        services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
        services.AddSingleton<IConstructionSiteRepository, InMemoryConstructionSiteRepository>();
        services.AddSingleton<ISiteAssignmentRepository, InMemorySiteAssignmentRepository>();
        services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
        services.AddSingleton<IMaterialBookingRepository, InMemoryMaterialBookingRepository>();
        services.AddSingleton<IWorkLogRepository, InMemoryWorkLogRepository>();
        services.AddSingleton<IPhotoDocumentationRepository, InMemoryPhotoDocumentationRepository>();
        services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        services.AddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();

        services.AddSingleton<IFileStorageService>(_ => new LocalFileStorageService(storageRootPath));

        return services;
    }
}
