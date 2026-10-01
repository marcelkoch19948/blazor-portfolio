#nullable enable

namespace BauToolKit.Domain.Models;

/// <summary>
/// Auftraggeber oder Kunde eines Bauprojekts.
/// </summary>
public class Customer
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? ContactPerson { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public string FullAddress => $"{Street}, {ZipCode} {City}".Trim(',', ' ');
}
