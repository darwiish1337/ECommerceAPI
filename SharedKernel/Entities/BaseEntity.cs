namespace SharedKernel.Entities;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    private readonly int _cachedHashCode;

    protected BaseEntity()
    {
        _cachedHashCode = 0; // fallback for EF/Dapper
    }

    protected BaseEntity(TId id)
    {
        Id = id;
        _cachedHashCode = EqualityComparer<TId>.Default.GetHashCode(Id!);
    }

    public override bool Equals(object? obj)
    {
        return obj is BaseEntity<TId> entity && EqualityComparer<TId>.Default.Equals(Id, entity.Id);
    }

    public override int GetHashCode() => _cachedHashCode;
}