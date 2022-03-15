namespace Bank98Functions.Transactions;

public static class TransactionExtensions
{
    public static TransactionEntity ToTable(this Transaction transaction)
    {
        return new TransactionEntity
        {
            PartitionKey = "TRANSACTION",
            RowKey = transaction.Id,
            ExecutionDate = transaction.ExecutionDate,
            Description = transaction.Description,
            Amount = transaction.Amount,
            CreditorIBAN = transaction.CreditorIBAN,
            DebtorIBAN = transaction.DebtorIBAN
        };
    }

    public static Transaction ToTransaction(this TransactionEntity transactionTable)
    {
        return new Transaction
        {
            Id = transactionTable.RowKey,
            Amount = transactionTable.Amount,
            CreditorIBAN = transactionTable.CreditorIBAN,
            DebtorIBAN = transactionTable.DebtorIBAN,
            ExecutionDate = transactionTable.ExecutionDate,
            Description = transactionTable.Description,
        };
    }
}
