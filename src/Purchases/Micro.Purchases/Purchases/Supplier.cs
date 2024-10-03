using Micro.Core.Common.Entities;

namespace Micro.Purchases.Purchases;

internal class Supplier : BaseEntity
{
    public Supplier(string name, string email, string document, SupplierAddress address) 
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue)
    {
        Name = name;
        Email = email;
        Document = document;
        Address = address;
    }
    
    public Supplier(Guid id, string name, string email, string document, SupplierAddress address,
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

    public SupplierTypeEnum SupplierType { get; private set; }
    
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

    public SupplierAddress Address { get; private set; }
}