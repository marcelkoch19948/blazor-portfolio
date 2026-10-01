#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BauToolKit.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBauToolKitApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IConstructionSiteService, ConstructionSiteService>();
        services.AddScoped<ISiteAssignmentService, SiteAssignmentService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IWorkLogService, WorkLogService>();
        services.AddScoped<IPhotoDocumentationService, PhotoDocumentationService>();
        services.AddScoped<IInvoiceService, InvoiceService>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        => services.AddBauToolKitApplication();
}
