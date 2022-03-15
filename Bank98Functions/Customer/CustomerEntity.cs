using Microsoft.Azure.Cosmos.Table;
using System;

namespace Bank98Functions.Customer;

public class CustomerEntity : TableEntity
{
    public DateTime CreatedTime { get; set; }
    public string Name { get; set; }

    public string Address { get; set; }

    public string IBAN { get; set; }
}
