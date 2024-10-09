using Micro.Core.Common;
using Micro.Core.Common.Entities;

namespace Micro.Finances.Common.Account;

internal class Account : SoftDeletableEntity
{
    public Account(string name, AccountTypeEnum accountType, decimal currentBalance, bool active) 
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue, false)
    {
        Name = name;
        AccountType = accountType;
        CurrentBalance = currentBalance;
        Active = active;
    }
    
    public Account(Guid id, string name, AccountTypeEnum accountType, decimal currentBalance, bool active,
        DateTime createdAt, DateTime updatedAt, bool isIsDeleted) 
        : base(id, createdAt, updatedAt, isIsDeleted)
    {
        Name = name;
        AccountType = accountType;
        CurrentBalance = currentBalance;
        Active = active;
    }
    
    private string _name;
    public string Name
    {
        get => _name ??= "";
        private set => _name = value ?? "";
    }

    public AccountTypeEnum AccountType { get; private set; }

    public decimal CurrentBalance { get; private set; }

    public bool Active { get; private set; }
}