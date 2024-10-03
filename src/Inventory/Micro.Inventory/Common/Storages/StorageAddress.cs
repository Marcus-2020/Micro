namespace Micro.Inventory.Common.Storages;

internal record struct StorageAddress(
    Guid Id,
    Guid StorageId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    string Number,
    bool IsMainAddress);