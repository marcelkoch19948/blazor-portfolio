#nullable enable

using BauToolKit.Application;
using BauToolKit.Application.DTOs;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;
using BauToolKit.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddBauToolKitApplication();
builder.Services.AddBauToolKitInfrastructure();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

// Status endpoint
app.MapGet("/api/status", () => Results.Ok(new
{
    Application = "BauToolKit.Api",
    Status = "Healthy",
    Version = "1.0.0"
}));

// Customer endpoints
app.MapGet("/api/customers", async (ICustomerService customerService, CancellationToken ct) =>
{
    IReadOnlyList<Customer> customers = await customerService.GetAllAsync(ct);
    return Results.Ok(customers);
});

app.MapGet("/api/customers/{id:guid}", async (Guid id, ICustomerService customerService, CancellationToken ct) =>
{
    Customer? customer = await customerService.GetByIdAsync(id, ct);
    return customer is not null ? Results.Ok(customer) : Results.NotFound();
});

app.MapPost("/api/customers", async (Customer customer, ICustomerService customerService, CancellationToken ct) =>
{
    Customer created = await customerService.CreateAsync(customer, ct);
    return Results.Created($"/api/customers/{created.Id}", created);
});

// Construction site endpoints
app.MapGet("/api/sites", async (IConstructionSiteService siteService, CancellationToken ct) =>
{
    IReadOnlyList<ConstructionSite> sites = await siteService.GetAllAsync(ct);
    return Results.Ok(sites);
});

app.MapGet("/api/sites/{id:guid}", async (Guid id, IConstructionSiteService siteService, CancellationToken ct) =>
{
    ConstructionSite? site = await siteService.GetByIdAsync(id, ct);
    return site is not null ? Results.Ok(site) : Results.NotFound();
});

app.MapPost("/api/sites", async (ConstructionSite site, IConstructionSiteService siteService, CancellationToken ct) =>
{
    ConstructionSite created = await siteService.CreateAsync(site, ct);
    return Results.Created($"/api/sites/{created.Id}", created);
});

app.MapPut("/api/sites/{id:guid}/progress", async (
    Guid id,
    UpdateProgressRequest request,
    IConstructionSiteService siteService,
    CancellationToken ct) =>
{
    ConstructionSite updated = await siteService.UpdateProgressAsync(id, request.ProgressPercentage, ct);
    return Results.Ok(updated);
});

// Site Assignment endpoints
app.MapGet("/api/sites/{siteId:guid}/assignments", async (
    Guid siteId,
    ISiteAssignmentService assignmentService,
    CancellationToken ct) =>
{
    IReadOnlyList<SiteAssignment> assignments = await assignmentService.GetBySiteIdAsync(siteId, ct);
    return Results.Ok(assignments);
});

app.MapGet("/api/users/{userId:guid}/assignments", async (
    Guid userId,
    ISiteAssignmentService assignmentService,
    CancellationToken ct) =>
{
    IReadOnlyList<SiteAssignment> assignments = await assignmentService.GetByUserIdAsync(userId, ct);
    return Results.Ok(assignments);
});

app.MapPost("/api/sites/assign", async (
    AssignUserRequest request,
    ISiteAssignmentService assignmentService,
    CancellationToken ct) =>
{
    SiteAssignment assignment = await assignmentService.AssignEmployeeAsync(
        request.SiteId,
        request.UserId,
        request.AssignedByUserId,
        request.Notes,
        ct);
    return Results.Created($"/api/sites/{request.SiteId}/assignments/{assignment.Id}", assignment);
});

// Inventory & Material endpoints
app.MapGet("/api/inventory", async (IInventoryService inventoryService, CancellationToken ct) =>
{
    IReadOnlyList<InventoryItem> items = await inventoryService.GetAllItemsAsync(ct);
    return Results.Ok(items);
});

app.MapGet("/api/inventory/low-stock", async (IInventoryService inventoryService, CancellationToken ct) =>
{
    IReadOnlyList<InventoryItem> items = await inventoryService.GetLowStockItemsAsync(ct);
    return Results.Ok(items);
});

app.MapPost("/api/inventory", async (InventoryItem item, IInventoryService inventoryService, CancellationToken ct) =>
{
    InventoryItem created = await inventoryService.CreateItemAsync(item, ct);
    return Results.Created($"/api/inventory/{created.Id}", created);
});

app.MapPost("/api/inventory/book", async (
    MaterialBookingRequest request,
    IInventoryService inventoryService,
    CancellationToken ct) =>
{
    MaterialBooking booking = await inventoryService.BookMaterialAsync(request, ct);
    return Results.Ok(booking);
});

// Work Log endpoints
app.MapGet("/api/sites/{siteId:guid}/worklogs", async (
    Guid siteId,
    IWorkLogService workLogService,
    CancellationToken ct) =>
{
    IReadOnlyList<WorkLog> logs = await workLogService.GetBySiteIdAsync(siteId, ct);
    return Results.Ok(logs);
});

app.MapGet("/api/users/{userId:guid}/worklogs", async (
    Guid userId,
    IWorkLogService workLogService,
    CancellationToken ct) =>
{
    IReadOnlyList<WorkLog> logs = await workLogService.GetByUserIdAsync(userId, ct);
    return Results.Ok(logs);
});

app.MapPost("/api/worklogs", async (
    WorkLogRequest request,
    IWorkLogService workLogService,
    CancellationToken ct) =>
{
    WorkLog created = await workLogService.LogHoursAsync(request, ct);
    return Results.Created($"/api/worklogs/{created.Id}", created);
});

// Photo Documentation endpoints
app.MapGet("/api/sites/{siteId:guid}/photos", async (
    Guid siteId,
    IPhotoDocumentationService photoService,
    CancellationToken ct) =>
{
    IReadOnlyList<PhotoDocumentation> photos = await photoService.GetBySiteIdAsync(siteId, ct);
    return Results.Ok(photos);
});

app.MapPost("/api/photos", async (
    HttpRequest httpRequest,
    IPhotoDocumentationService photoService,
    CancellationToken ct) =>
{
    if (!httpRequest.HasFormContentType)
    {
        return Results.BadRequest("Erwartet multipart/form-data mit Datei und Metadaten.");
    }

    IFormCollection form = await httpRequest.ReadFormAsync(ct);
    IFormFile? file = form.Files.GetFile("file") ?? form.Files.FirstOrDefault();
    if (file is null || file.Length == 0)
    {
        return Results.BadRequest("Es wurde keine Datei übermittelt.");
    }

    if (!Guid.TryParse(form["siteId"], out Guid siteId))
    {
        return Results.BadRequest("Ungültige oder fehlende Baustellen-ID (siteId).");
    }

    if (!Guid.TryParse(form["userId"], out Guid userId))
    {
        return Results.BadRequest("Ungültige oder fehlende Mitarbeiter-ID (userId).");
    }

    string? description = form["description"];

    await using Stream stream = file.OpenReadStream();
    PhotoUploadRequest request = new(
        ConstructionSiteId: siteId,
        UploadedByUserId: userId,
        FileName: file.FileName,
        Content: stream,
        Description: description
    );

    PhotoDocumentation created = await photoService.AddPhotoAsync(request, ct);
    return Results.Created($"/api/photos/{created.Id}", created);
}).DisableAntiforgery();

// Invoices
app.MapGet("/api/invoices", async (IInvoiceService invoiceService, CancellationToken ct) =>
{
    IReadOnlyList<Invoice> invoices = await invoiceService.GetAllAsync(ct);
    return Results.Ok(invoices);
});

app.MapGet("/api/sites/{siteId:guid}/invoices", async (Guid siteId, IInvoiceService invoiceService, CancellationToken ct) =>
{
    IReadOnlyList<Invoice> invoices = await invoiceService.GetBySiteIdAsync(siteId, ct);
    return Results.Ok(invoices);
});

app.MapGet("/api/invoices/{id:guid}", async (Guid id, IInvoiceService invoiceService, CancellationToken ct) =>
{
    Invoice? invoice = await invoiceService.GetByIdAsync(id, ct);
    return invoice is not null ? Results.Ok(invoice) : Results.NotFound();
});

app.MapPost("/api/invoices", async (Invoice invoice, IInvoiceService invoiceService, CancellationToken ct) =>
{
    Invoice created = await invoiceService.CreateInvoiceAsync(invoice, ct);
    return Results.Created($"/api/invoices/{created.Id}", created);
});

app.MapPatch("/api/invoices/{id:guid}/status", async (Guid id, UpdateInvoiceStatusRequest request, IInvoiceService invoiceService, CancellationToken ct) =>
{
    Invoice updated = await invoiceService.UpdateStatusAsync(id, request.Status, ct);
    return Results.Ok(updated);
});

await SampleDataSeeder.SeedAsync(app.Services);

app.Run();

public record UpdateProgressRequest(int ProgressPercentage);
public record AssignUserRequest(Guid SiteId, Guid UserId, Guid? AssignedByUserId, string? Notes);
public record UpdateInvoiceStatusRequest(InvoiceStatus Status);
