using Microsoft.Azure.Cosmos.Table;
using System;

namespace Bank98Functions.Transactions;

public class TransactionEntity : TableEntity
{
    public DateTime ExecutionDate { get; set; } = DateTime.UtcNow;

    public string Description { get; set; }

    public double Amount { get; set; }

    public string CreditorIBAN { get; set; }

    public string DebtorIBAN { get; set; }

}
