#nullable enable

using BauToolKit.Application.DTOs;
using BauToolKit.Application.Interfaces;
using BauToolKit.Domain.Models;

namespace BauToolKit.Application.Services;

public class WorkLogService(
    IWorkLogRepository workLogRepository,
    IConstructionSiteRepository siteRepository,
    IUserRepository userRepository) : IWorkLogService
{
    public async Task<IReadOnlyList<WorkLog>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        return await workLogRepository.GetBySiteIdAsync(siteId, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await workLogRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<WorkLog> LogHoursAsync(WorkLogRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.ActivityDescription))
        {
            throw new ArgumentException("Tätigkeitsbeschreibung darf nicht leer sein.", nameof(request));
        }

        ConstructionSite? site = await siteRepository.GetByIdAsync(request.ConstructionSiteId, cancellationToken);
        if (site is null)
        {
            throw new KeyNotFoundException($"Baustelle mit Id '{request.ConstructionSiteId}' wurde nicht gefunden.");
        }

        User? user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new KeyNotFoundException($"Mitarbeiter mit Id '{request.UserId}' wurde nicht gefunden.");
        }

        var workLog = new WorkLog
        {
            UserId = request.UserId,
            ConstructionSiteId = request.ConstructionSiteId,
            Date = request.Date,
            ActivityDescription = request.ActivityDescription
        };

        workLog.SetHours(request.Hours);

        return await workLogRepository.AddAsync(workLog, cancellationToken);
    }

    public async Task<WorkLog> ApproveHoursAsync(Guid workLogId, CancellationToken cancellationToken = default)
    {
        WorkLog? log = await workLogRepository.GetByIdAsync(workLogId, cancellationToken);
        if (log is null)
        {
            throw new KeyNotFoundException($"Stundeneintrag mit Id '{workLogId}' wurde nicht gefunden.");
        }

        log.Approve();
        return await workLogRepository.UpdateAsync(log, cancellationToken);
    }

    public async Task<decimal> GetTotalHoursForSiteAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<WorkLog> logs = await workLogRepository.GetBySiteIdAsync(siteId, cancellationToken);
        return logs.Sum(l => l.Hours);
    }
}
