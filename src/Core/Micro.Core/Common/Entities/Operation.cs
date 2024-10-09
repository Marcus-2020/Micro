using Micro.Core.Common.Enums;

namespace Micro.Core.Common.Entities;

public class Operation : SoftDeletableEntity
{
    public Operation(string name, InboudOutboundEnum inboudOutbound, bool active)
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue, false)
    {
        Name = name;
        InboudOutbound = inboudOutbound;
        Active = active;
    }
    
    public Operation(Guid id, string name, InboudOutboundEnum inboudOutbound, bool active,
        DateTime createdAt, DateTime updatedAt, bool isIsDeleted) 
        : base(id, createdAt, updatedAt, isIsDeleted)
    {
        Name = name;
        InboudOutbound = inboudOutbound;
        Active = active;
    }
    
    private string _name;
    public string Name
    {
        get => _name ??= "";
        private set => _name = value ?? "";
    }
    
    public InboudOutboundEnum InboudOutbound { get; private set; }

    public bool Active { get; set; }
}