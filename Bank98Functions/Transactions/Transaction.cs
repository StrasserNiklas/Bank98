using System;

namespace Bank98Functions.Transactions;

public class Transaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime ExecutionDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; }

    public double Amount { get; set; }

    public string CreditorIBAN { get; set; }

    public string DebtorIBAN { get; set; }

}