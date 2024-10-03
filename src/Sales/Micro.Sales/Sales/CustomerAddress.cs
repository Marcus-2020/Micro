namespace Micro.Sales.Sales;

internal record struct CustomerAddress(
    Guid CustomerId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    string Number);