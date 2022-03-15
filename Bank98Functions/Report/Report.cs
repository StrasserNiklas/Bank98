using Bank98Functions.Transactions;
using System.Collections.Generic;

namespace Bank98Functions.Report;

public class Report
{
    public List<Transaction> Transactions { get; set; }

    public Customer.Customer Customer { get; set; }
}
