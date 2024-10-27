using Micro.Core.Common.Entities;

namespace Micro.Finances.Common.Receivable;

internal class Receivable : SoftDeletableEntity
{
    public Receivable(Guid accountId, Guid operationId, Guid beneficiaryId, string description, DateTime dueDate,
        DateTime paidDate, decimal totalValue, decimal valuePaid)
        : base(Guid.Empty, DateTime.MinValue, DateTime.MinValue)
    {
        AccountId = accountId;
        OperationId = operationId;
        BeneficiaryId = beneficiaryId;
        Description = description;
        DueDate = dueDate;
        PaidDate = paidDate;
        TotalValue = totalValue;
        ValuePaid = valuePaid;
    }
    
    public Receivable(Guid id, Guid accountId, Guid operationId, Guid beneficiaryId, string description, DateTime dueDate,
        DateTime paidDate, decimal totalValue, decimal valuePaid, DateTime createdAt, DateTime updatedAt, bool isIsDeleted)
        : base(id, createdAt, updatedAt)
    {
        AccountId = accountId;
        OperationId = operationId;
        BeneficiaryId = beneficiaryId;
        Description = description;
        DueDate = dueDate;
        PaidDate = paidDate;
        TotalValue = totalValue;
        ValuePaid = valuePaid;
    }
    
    public Guid AccountId { get; private set; }
    
    public Guid OperationId { get; private set; }
    
    public Guid BeneficiaryId { get; private set; }
    
    private string _description;
    public string Description
    {
        get => _description ??= "";
        private set => _description = value ?? "";
    }

    public DateTime DueDate { get; private set; }
    
    public DateTime PaidDate { get; private set; }

    public decimal TotalValue { get; private set; }
    public decimal ValuePaid { get; private set; }

    public decimal ValueUnpaid => TotalValue - ValuePaid;

    public bool IsPaid => TotalValue == ValuePaid;
    public bool IsPartiallyPaid => ValuePaid > 0;
    public bool IsDue(DateTime today) => today > DueDate;

    public bool Pay(decimal paymentValue, DateTime today)
    {
        if (paymentValue <= 0) return false;
        if (paymentValue > ValueUnpaid) return false;
        if (IsDue(today)) return false;

        ValuePaid += paymentValue;
        return true;
    }

    public bool ChangeDueDate(DateTime newDate, DateTime today)
    {
        if (newDate < today) return false;
        if (newDate <= DueDate) return false;
        DueDate = newDate;
        return true;
    }
}