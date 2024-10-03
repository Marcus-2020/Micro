using Micro.Core.Common.Entities;

namespace Micro.Sales.Sales;

internal class Customer : BaseEntity
{
    public Customer(string name, string email, string document, CustomerAddress address) 
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue)
    {
        Name = name;
        Email = email;
        Document = document;
        Address = address;
    }
    
    public Customer(Guid id, string name, string email, string document, CustomerAddress address,
        DateTime createdAt, DateTime updatedAt) : base(id, createdAt, updatedAt)
    {
        Name = name;
        Email = email;
        Document = document;
        Address = address;
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
}