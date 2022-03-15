namespace Bank98Functions.Transactions;

public class TransactionDto
{
    public string Description { get; set; }

    public double Amount { get; set; }

    public string CreditorIBAN { get; set; }

    public string DebtorIBAN { get; set; }

}
