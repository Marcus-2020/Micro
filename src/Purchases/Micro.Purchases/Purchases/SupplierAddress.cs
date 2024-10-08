namespace Micro.Purchases.Purchases;

internal record struct SupplierAddress(
    Guid Id,
    Guid CustomerId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    string Number,
    bool IsMainAddress);