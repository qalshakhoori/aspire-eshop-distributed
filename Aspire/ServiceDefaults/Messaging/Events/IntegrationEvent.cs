using System;

namespace ServiceDefaults.Messaging.Events;

// Base class for integration events
// Using events folder in Service Defaults is not recommended for production scenarios
public record IntegrationEvent
{
    public Guid EventId => Guid.NewGuid();
    public DateTime EventCreatedAt => DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName ?? "Unknown";
}
