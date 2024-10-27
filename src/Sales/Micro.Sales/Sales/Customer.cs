using Micro.Core.Common.Entities;

namespace Micro.Sales.Sales;

internal class Customer : SoftDeletableEntity
{
    public Customer(string name, string email, string document, CustomerAddress address, bool active) 
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue)
    {
        Name = name;
        Email = email;
        Document = document;
        Address = address;
        Active = active;
    }
    
    public Customer(Guid id, string name, string email, string document, CustomerAddress address, bool active,
        DateTime createdAt, DateTime updatedAt, bool isDeleted) : base(id, createdAt, updatedAt)
    {
        Name = name;
        Email = email;
        Document = document;
        Address = address;
        Active = active;
    }

    private string _name;
    public string Name
    {
        get => _name ??= "";
        private set => _name = value ?? "";
    }

    public CustomerTypeEnum CustomerType { get; private set; }
    
    private string _email;
    public string Email
    {
        get => _email ??= "";
        private set => _email = value ?? "";
    }
    
    private string _document;
    public string Document
    {
        get => _document??= "";
        private set => _document = value ?? "";
    }

    public CustomerAddress Address { get; private set; }
    
    public bool Active { get; private set; }
}