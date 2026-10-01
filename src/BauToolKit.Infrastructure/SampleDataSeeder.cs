#nullable enable

using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Enums;
using BauToolKit.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BauToolKit.Infrastructure;

public static class SampleDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        IUserRepository userRepo = serviceProvider.GetRequiredService<IUserRepository>();
        ICustomerRepository customerRepo = serviceProvider.GetRequiredService<ICustomerRepository>();
        IConstructionSiteRepository siteRepo = serviceProvider.GetRequiredService<IConstructionSiteRepository>();
        ISiteAssignmentRepository assignmentRepo = serviceProvider.GetRequiredService<ISiteAssignmentRepository>();
        IInventoryRepository inventoryRepo = serviceProvider.GetRequiredService<IInventoryRepository>();
        IMaterialBookingRepository bookingRepo = serviceProvider.GetRequiredService<IMaterialBookingRepository>();
        IWorkLogRepository workLogRepo = serviceProvider.GetRequiredService<IWorkLogRepository>();
        IInvoiceRepository invoiceRepo = serviceProvider.GetRequiredService<IInvoiceRepository>();

        IReadOnlyList<User> existingUsers = await userRepo.GetAllAsync(cancellationToken);
        if (existingUsers.Count > 0)
        {
            return;
        }

        // 1. Benutzer (Chef & Mitarbeiter)
        User chef = new()
        {
            FirstName = "Thomas",
            LastName = "Meister",
            Email = "thomas.meister@bau-toolkit.de",
            Role = UserRole.Chef,
            IsActive = true
        };
        await userRepo.AddAsync(chef, cancellationToken);

        User mitarbeiter1 = new()
        {
            FirstName = "Michael",
            LastName = "Bauer",
            Email = "michael.bauer@bau-toolkit.de",
            Role = UserRole.Mitarbeiter,
            IsActive = true
        };
        await userRepo.AddAsync(mitarbeiter1, cancellationToken);

        User mitarbeiter2 = new()
        {
            FirstName = "Stefan",
            LastName = "Müller",
            Email = "stefan.mueller@bau-toolkit.de",
            Role = UserRole.Mitarbeiter,
            IsActive = true
        };
        await userRepo.AddAsync(mitarbeiter2, cancellationToken);

        // 2. Kunden
        Customer kunde1 = new()
        {
            Name = "Familie Schmidt",
            ContactPerson = "Sabine Schmidt",
            Email = "schmidt@beispiel-familie.de",
            PhoneNumber = "+49 30 123456",
            Street = "Musterweg 12",
            ZipCode = "10115",
            City = "Berlin"
        };
        await customerRepo.AddAsync(kunde1, cancellationToken);

        Customer kunde2 = new()
        {
            Name = "Gewerbepark Süd GmbH",
            CompanyName = "Gewerbepark Süd GmbH & Co. KG",
            ContactPerson = "Klaus Weber (Bauleiter)",
            Email = "weber@gewerbepark-sued.de",
            PhoneNumber = "+49 89 987654",
            Street = "Industriestr. 45",
            ZipCode = "80331",
            City = "München"
        };
        await customerRepo.AddAsync(kunde2, cancellationToken);

        // 3. Baustellen
        ConstructionSite site1 = new()
        {
            CustomerId = kunde1.Id,
            Title = "Neubau Einfamilienhaus Schmidt",
            Description = "Errichtung eines energieeffizienten KfW-40 Einfamilienhauses inkl. Bodenplatte und Dach.",
            Street = "Musterweg 12",
            ZipCode = "10115",
            City = "Berlin",
            Status = ConstructionSiteStatus.InArbeit,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
            TargetCompletionDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60))
        };
        site1.UpdateProgress(45);
        await siteRepo.AddAsync(site1, cancellationToken);

        ConstructionSite site2 = new()
        {
            CustomerId = kunde2.Id,
            Title = "Dachsanierung Halle 3",
            Description = "Komplette Erneuerung der Eindeckung und Wärmedämmung der Gewerbehalle 3.",
            Street = "Industriestr. 45",
            ZipCode = "80331",
            City = "München",
            Status = ConstructionSiteStatus.InArbeit,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-15)),
            TargetCompletionDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20))
        };
        site2.UpdateProgress(70);
        await siteRepo.AddAsync(site2, cancellationToken);

        ConstructionSite site3 = new()
        {
            CustomerId = kunde1.Id,
            Title = "Gartengestaltung & Pflasterarbeiten",
            Description = "Anlegen der Garagenzufahrt und Terrasse mit Pflastersteinen.",
            Street = "Musterweg 12",
            ZipCode = "10115",
            City = "Berlin",
            Status = ConstructionSiteStatus.Geplant,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
            TargetCompletionDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(50))
        };
        await siteRepo.AddAsync(site3, cancellationToken);

        // 4. Baustellen-Zuweisungen
        SiteAssignment assign1 = new()
        {
            ConstructionSiteId = site1.Id,
            UserId = mitarbeiter1.Id,
            AssignedByUserId = chef.Id,
            Notes = "Bauleiter vor Ort / Rohbauleitung",
            IsActive = true
        };
        await assignmentRepo.AddAsync(assign1, cancellationToken);

        SiteAssignment assign2 = new()
        {
            ConstructionSiteId = site1.Id,
            UserId = mitarbeiter2.Id,
            AssignedByUserId = chef.Id,
            Notes = "Unterstützung Fundament und Mauerwerk",
            IsActive = true
        };
        await assignmentRepo.AddAsync(assign2, cancellationToken);

        SiteAssignment assign3 = new()
        {
            ConstructionSiteId = site2.Id,
            UserId = mitarbeiter2.Id,
            AssignedByUserId = chef.Id,
            Notes = "Dacharbeiten Halle 3",
            IsActive = true
        };
        await assignmentRepo.AddAsync(assign3, cancellationToken);

        // 5. Lagerartikel
        InventoryItem item1 = new()
        {
            ItemNumber = "MAT-001",
            Name = "Portlandzement CEM II 25kg",
            Description = "Hochwertiger Portlandkompositzement für Beton- und Estricharbeiten.",
            Unit = "Sack",
            MinimumStock = 20m,
            UnitPrice = 7.45m
        };
        item1.AdjustStock(85m);
        await inventoryRepo.AddAsync(item1, cancellationToken);

        InventoryItem item2 = new()
        {
            ItemNumber = "MAT-002",
            Name = "Dachziegel Braas Rubin 11V",
            Description = "Tondachziegel naturrot.",
            Unit = "Stk",
            MinimumStock = 500m,
            UnitPrice = 2.15m
        };
        item2.AdjustStock(1400m);
        await inventoryRepo.AddAsync(item2, cancellationToken);

        InventoryItem item3 = new()
        {
            ItemNumber = "MAT-003",
            Name = "Universalschrauben 5x60mm (200 Stk)",
            Description = "Torx TX25, verzinkt mit Senkkopf.",
            Unit = "Paket",
            MinimumStock = 10m,
            UnitPrice = 14.90m
        };
        item3.AdjustStock(42m);
        await inventoryRepo.AddAsync(item3, cancellationToken);

        InventoryItem item4 = new()
        {
            ItemNumber = "MAT-004",
            Name = "Gipskartonplatten 2000x1250x12.5mm",
            Description = "Bauplatten für den trockenen Innenausbau.",
            Unit = "Stk",
            MinimumStock = 15m,
            UnitPrice = 8.80m
        };
        item4.AdjustStock(30m);
        await inventoryRepo.AddAsync(item4, cancellationToken);

        InventoryItem item5 = new()
        {
            ItemNumber = "MAT-005",
            Name = "Kabel NYM-J 3x1.5mm² (100m)",
            Description = "Mantelleitung für Feucht- und Trockenräume.",
            Unit = "Rolle",
            MinimumStock = 5m,
            UnitPrice = 59.00m
        };
        item5.AdjustStock(8m);
        await inventoryRepo.AddAsync(item5, cancellationToken);

        // 6. Materialbuchungen
        MaterialBooking booking1 = new()
        {
            InventoryItemId = item1.Id,
            ConstructionSiteId = site1.Id,
            BookedByUserId = mitarbeiter1.Id,
            Type = BookingType.Verbrauch,
            Quantity = 10m,
            Notes = "10 Sack Zement für Fundamentverschalung"
        };
        await bookingRepo.AddAsync(booking1, cancellationToken);

        MaterialBooking booking2 = new()
        {
            InventoryItemId = item2.Id,
            ConstructionSiteId = site2.Id,
            BookedByUserId = mitarbeiter2.Id,
            Type = BookingType.Verbrauch,
            Quantity = 250m,
            Notes = "Erste Palette Dachziegel für Nordseite"
        };
        await bookingRepo.AddAsync(booking2, cancellationToken);

        // 7. Arbeitszeiteinträge
        WorkLog workLog1 = new()
        {
            ConstructionSiteId = site1.Id,
            UserId = mitarbeiter1.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            ActivityDescription = "Bewehrungsarbeiten und Fundamentverschalung fertiggestellt."
        };
        workLog1.SetHours(7.5m);
        await workLogRepo.AddAsync(workLog1, cancellationToken);

        WorkLog workLog2 = new()
        {
            ConstructionSiteId = site2.Id,
            UserId = mitarbeiter2.Id,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            ActivityDescription = "Alte Eindeckung abgetragen und Lattung kontrolliert."
        };
        workLog2.SetHours(8.0m);
        await workLogRepo.AddAsync(workLog2, cancellationToken);

        // 8. Rechnungen (Abschlagsrechnungen & Rechnungen)
        Invoice invoice1 = new()
        {
            InvoiceNumber = "RE-2026-001",
            ConstructionSiteId = site1.Id,
            CustomerId = kunde1.Id,
            Type = InvoiceType.Abschlagsrechnung,
            Status = InvoiceStatus.Gestellt,
            IssueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(9)),
            Notes = "1. Abschlagsrechnung nach Fertigstellung Bodenplatte"
        };
        invoice1.AddPosition(new InvoicePosition
        {
            InvoiceId = invoice1.Id,
            Description = "1. Bauabschnitt: Erdarbeiten und Bodenplatte",
            Quantity = 1m,
            Unit = "pauschal",
            UnitPrice = 8500.00m
        });
        invoice1.AddPosition(new InvoicePosition
        {
            InvoiceId = invoice1.Id,
            Description = "Materialpauschale Beton und Stahlbewehrung",
            Quantity = 1m,
            Unit = "pauschal",
            UnitPrice = 2400.00m
        });
        await invoiceRepo.AddAsync(invoice1, cancellationToken);

        Invoice invoice2 = new()
        {
            InvoiceNumber = "RE-2026-002",
            ConstructionSiteId = site2.Id,
            CustomerId = kunde2.Id,
            Type = InvoiceType.Abschlagsrechnung,
            Status = InvoiceStatus.Bezahlt,
            IssueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-14)),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            Notes = "1. Teilzahlung Gerüstbau und Demontage"
        };
        invoice2.AddPosition(new InvoicePosition
        {
            InvoiceId = invoice2.Id,
            Description = "Gerüststellung und Baustellensicherung",
            Quantity = 1m,
            Unit = "pauschal",
            UnitPrice = 1950.00m
        });
        invoice2.AddPosition(new InvoicePosition
        {
            InvoiceId = invoice2.Id,
            Description = "Demontage Altdach und fachgerechte Entsorgung",
            Quantity = 40m,
            Unit = "Std",
            UnitPrice = 65.00m
        });
        await invoiceRepo.AddAsync(invoice2, cancellationToken);
    }
}
