using Micro.Core.Common.Entities;
using Micro.Core.Common.Enums;

namespace Micro.Finances.Common.Movement;

internal class Movement : SoftDeletableEntity
{
    public Movement(Guid accountId, string description, InboudOutboundEnum inboudOutbound,
        MovementTypeEnum movementType, decimal value) 
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue, false)
    {
    }
    
    public Movement(Guid id, Guid accountId, string description, InboudOutboundEnum inboudOutbound,
        MovementTypeEnum movementType, decimal value, DateTime createdAt, DateTime updatedAt, bool isIsDeleted) 
        : base(id, createdAt, updatedAt, isIsDeleted)
    {
    }

    public Guid AccountId { get; private set; }
    
    private string _description;
    public string Description
    {
        get => _description ??= "";
        private set => _description = value ?? "";
    }

    public InboudOutboundEnum InboudOutbound { get; private set; }
    
    public MovementTypeEnum MovementType { get; private set; }

    public decimal Value { get; private set; }
}