namespace Micro.Core.Common.Entities;

public abstract class SoftDeletableEntity : BaseEntity
{
    protected SoftDeletableEntity(Guid id, DateTime createdAt, DateTime updatedAt, bool isIsDeleted) : base(id, createdAt, updatedAt)
    {
        IsDeleted = isIsDeleted;
    }

    public bool IsDeleted { get; private set; }

    public void SetAsDeleted()
    {
        if (!IsDeleted) IsDeleted = true;
    }
}