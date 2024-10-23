namespace Micro.Core.Common.Entities;

public abstract class SoftDeletableEntity : BaseEntity
{
    protected SoftDeletableEntity(Guid id, DateTime createdAt, DateTime? updatedAt, bool isIsDeleted, DateTime? deletedAt = null) : base(id, createdAt, updatedAt)
    {
        IsDeleted = isIsDeleted;
        DeletedAt = deletedAt;
    }

    public DateTime? DeletedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public void SetAsDeleted()
    {
        if (!IsDeleted) IsDeleted = true;
    }
}