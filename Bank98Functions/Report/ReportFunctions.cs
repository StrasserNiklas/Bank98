using Bank98Functions.Customer;
using Bank98Functions.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bank98Functions.Report;

public class ReportFunctions
{
    [FunctionName("GetReport")]
    public static async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "report")] HttpRequest req,
        [Table("Transaction", Connection = "AzureWebJobsStorage")] CloudTable transactionTable,
        [Table("Customers", Connection = "AzureWebJobsStorage")] CloudTable customerTable,
        ILogger log)
    {
        string iban = req.Query["iban"];
        string time = req.Query["month"];
        if (!DateTime.TryParse(time, out var timeToFetch) || string.IsNullOrWhiteSpace(iban))
        {
            return new BadRequestObjectResult("Please provide all neccessary and correct information.");
        }

        var operation = TableOperation.Retrieve<CustomerEntity>("CUSTOMER", iban);
        var customer = (await customerTable.ExecuteAsync(operation)).Result as CustomerEntity;

        var ibanFilter = TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("CreditorIBAN", QueryComparisons.Equal, iban),
                                                   TableOperators.Or,
                                                   TableQuery.GenerateFilterCondition("DebtorIBAN", QueryComparisons.Equal, iban));

        //var timeFilter = TableQuery.CombineFilters(TableQuery.GenerateFilterConditionForDate("ExecutionDate", QueryComparisons.GreaterThanOrEqual, timeToFetch),
        //                                           TableOperators.And,
        //                                           TableQuery.GenerateFilterConditionForDate("DebtorIBAN", QueryComparisons.LessThanOrEqual, timeToFetch.AddMonths(1)));

        //var finalFilter = TableQuery.CombineFilters(ibanFilter, TableOperators.And, timeFilter);

        var query = new TableQuery<TransactionEntity>()
            .Where(ibanFilter);

        var segment = await transactionTable.ExecuteQuerySegmentedAsync(query, null);
        var data = segment.Select(TransactionExtensions.ToTransaction);

        var result = data
            .Where(transaction => transaction.ExecutionDate.Month == timeToFetch.Month && transaction.ExecutionDate.Year == timeToFetch.Year);

        return new OkObjectResult(new Report() { Customer = customer.ToCustomer(), Transactions = result.ToList() });
    }
}
