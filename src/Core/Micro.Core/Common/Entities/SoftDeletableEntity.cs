namespace Micro.Core.Common.Entities;

public abstract class SoftDeletableEntity : BaseEntity
{
    protected SoftDeletableEntity(Guid id, DateTime createdAt, DateTime? updatedAt, DateTime? deletedAt = null) : base(id, createdAt, updatedAt)
    {
        DeletedAt = deletedAt;
    }

    public DateTime? DeletedAt { get; private set; }
    public bool IsDeleted => DeletedAt is not null;

    public const int DeletedAtNoError = 0;
    public const int ErrorDeletedAtAlreadySet = 1;
    public const int ErrorDeletedAtNotInitialized = 2;
    public const int ErrorDeletedAtAfterCurrentDate = 3;

    public (bool IsSuccess, (int Code, string? Message) Error) SetDeletedAt(DateTime deletedAt)
    {
        if (DeletedAt is not null && DeletedAt > DateTime.MinValue)
        {
            return (false, (ErrorDeletedAtAlreadySet, "The date/time of deletion already set"));
        }
        
        if (deletedAt == DateTime.MinValue)
        {
            return (false, (ErrorDeletedAtNotInitialized, "The date/time of deletion needs to be initialized"));
        }

        if (deletedAt > DateTime.UtcNow)
        {
            return (false, (ErrorDeletedAtAfterCurrentDate, "The date/time of deletion can't be after the current date/time"));
        }

        DeletedAt = deletedAt;
        return (true, (DeletedAtNoError, null));
    }
}