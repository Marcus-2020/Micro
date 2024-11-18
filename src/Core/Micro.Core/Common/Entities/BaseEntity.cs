namespace Micro.Core.Common.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    protected BaseEntity(Guid id, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public const int IdNoError = 0;
    public const int ErrorIdAlreadySet = 1;
    
    public (bool IsSuccess, (int Code, string? Message) Error) SetId(Guid id)
    {
        if (Id != Guid.Empty)
        {
            return (false, (ErrorIdAlreadySet, "Id is already set"));
        }
        
        Id = id;
        return (true, (IdNoError, null));
    }
    
    
    public const int CreatedAtNoError = 0;
    public const int ErrorCreatedAtAlreadySet = 1;
    public const int ErrorCreatedAtNotInitialized = 2;
    public const int ErrorCreatedAtAfterCurrentDate = 3;
        
    public (bool IsSuccess, (int Code, string? Message) Error) SetCreatedAt(DateTime createdAt)
    {
        if (CreatedAt > DateTime.MinValue)
        {
            return (false, (ErrorCreatedAtAlreadySet, "The date/time of creation is already set"));
        }
        
        if (createdAt == DateTime.MinValue)
        {
            return (false, (ErrorCreatedAtNotInitialized, "The date/time of creation needs to be initialized"));
        }

        if (createdAt > DateTime.UtcNow)
        {
            return (false, (ErrorCreatedAtAfterCurrentDate, "The date/time of creation can't be after the current date/time"));
        }

        CreatedAt = createdAt;
        return (true, (CreatedAtNoError, null));
    }
}