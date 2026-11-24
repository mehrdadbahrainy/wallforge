using System.Collections.Concurrent;

namespace WallForge.Api;

public sealed record AuditEntry(DateTimeOffset When, string Event, string Actor);

/// <summary>Bounded activity view; also logs structured events to systemd journal via ILogger.</summary>
public sealed class AuditStore(ILogger<AuditStore> logger)
{
    private const int Limit = 250;
    private readonly ConcurrentQueue<AuditEntry> _events = new();

    public void Record(string action, string actor)
    {
        var entry = new AuditEntry(DateTimeOffset.UtcNow, action, actor);
        _events.Enqueue(entry);
        while (_events.Count > Limit && _events.TryDequeue(out _)) { }
        logger.LogInformation("WallForge audit event {Action} by {Actor}", action, actor);
    }

    public AuditEntry[] Recent() => _events.Reverse().ToArray();
}
