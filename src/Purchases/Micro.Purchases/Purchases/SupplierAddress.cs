namespace Micro.Purchases.Purchases;

internal record struct SupplierAddress(
    Guid CustomerId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    string Number);