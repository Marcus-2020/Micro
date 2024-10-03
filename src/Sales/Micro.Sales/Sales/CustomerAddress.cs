namespace Micro.Sales.Sales;

internal record struct CustomerAddress(
    Guid Id,
    Guid CustomerId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    string Number,
    bool IsMainAddress);