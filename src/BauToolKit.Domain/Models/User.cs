#nullable enable

using BauToolKit.Domain.Enums;

namespace BauToolKit.Domain.Models;

/// <summary>
/// Repräsentiert einen Benutzer im BauToolKit-System (Chef oder Mitarbeiter).
/// Hinweis: Passwörter und Authentifizierungsdetails werden von ASP.NET Core Identity / externen Identity-Providern
/// verwaltet und sind bewusst nicht Teil dieses Domänenmodells.
/// </summary>
public class User
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Mitarbeiter;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public bool IsChef => Role == UserRole.Chef;
    public bool IsMitarbeiter => Role == UserRole.Mitarbeiter;
}
