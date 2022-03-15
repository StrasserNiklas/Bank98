using System;

namespace Bank98Functions.Customer;

public class Customer
{
    public string Id { get; set; }

    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

    public string Name { get; set; }

    public string Address { get; set; }

    public string IBAN { get; set; }
}
