using System.ComponentModel.DataAnnotations;

namespace Execution.Domain.Common;

public interface ITenantScoped
{
    Guid TenantId { get; set; }
}

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAtUtc { get; set; }
}

public abstract class AuditableEntity
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedByUserId { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedByUserId { get; set; }

    // Concurrence optimiste — évite les écrasements silencieux
    // sur des updates concurrents (ex: 2 utilisateurs modifient le même TestCase)
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}

// Base commune : toute entité métier de TesterLab en hérite
public abstract class TenantAuditableEntity : AuditableEntity, ITenantScoped, ISoftDeletable
{
    public int Id { get; set; }

    // Guid exposé publiquement (API, URLs) — jamais l'Id interne.
    // Empêche l'énumération d'IDs séquentiels (IDOR) depuis l'extérieur.
    public Guid PublicId { get; set; } = Guid.NewGuid();

    public Guid TenantId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAtUtc { get; set; }
}