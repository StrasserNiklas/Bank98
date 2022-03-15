namespace Bank98Functions.Customer;

public static class CustomerExtensions
{
    public static CustomerEntity ToTable(this Customer customer)
    {
        return new CustomerEntity
        {
            PartitionKey = "CUSTOMER",
            RowKey = customer.IBAN,
            CreatedTime = customer.CreatedTime,
            Address = customer.Address,
            IBAN = customer.IBAN,
            Name = customer.Name
        };
    }

    public static Customer ToCustomer(this CustomerEntity customerTable)
    {
        return new Customer
        {
            Id = customerTable.RowKey,
            CreatedTime = customerTable.CreatedTime,
            Name = customerTable.Name,
            Address = customerTable.Address,
            IBAN = customerTable.IBAN,
        };
    }
}
