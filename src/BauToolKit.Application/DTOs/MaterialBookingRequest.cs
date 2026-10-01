#nullable enable

using BauToolKit.Domain.Enums;

namespace BauToolKit.Application.DTOs;

public record MaterialBookingRequest(
    Guid InventoryItemId,
    Guid? ConstructionSiteId,
    Guid BookedByUserId,
    BookingType Type,
    decimal Quantity,
    string? Notes = null);
