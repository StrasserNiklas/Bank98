using Bank98Functions.Customer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Bank98Functions.Transactions;

public class TransactionsFunctions
{
    [FunctionName("CreateTransaction")]
    public static async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "transaction")] HttpRequest req,
        [Table("Transaction", Connection = "AzureWebJobsStorage")] IAsyncCollector<TransactionEntity> transactionTableCollector,
        [Table("Customers", Connection = "AzureWebJobsStorage")] CloudTable customerTable,
        ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<TransactionDto>(requestBody);

        if (string.IsNullOrWhiteSpace(input.CreditorIBAN) || string.IsNullOrWhiteSpace(input.DebtorIBAN) || input.Amount <= 0)
        {
            return new BadRequestObjectResult("Please provide all neccessary information. Amount must be over 0.");
        }

        var operationDebtor = TableOperation.Retrieve<CustomerEntity>("CUSTOMER", input.DebtorIBAN);
        var customerDebtor = (await customerTable.ExecuteAsync(operationDebtor)).Result as CustomerEntity;

        var operationCreditor = TableOperation.Retrieve<CustomerEntity>("CUSTOMER", input.CreditorIBAN);
        var customerCreditor = (await customerTable.ExecuteAsync(operationCreditor)).Result as CustomerEntity;

        if (customerDebtor is null) return new BadRequestObjectResult("Debtor IBAN does not exist");
        if (customerCreditor is null) return new BadRequestObjectResult("Creditor IBAN does not exist");

        var transaction = new Transaction
        {
            Amount = input.Amount,
            CreditorIBAN = input.CreditorIBAN,
            DebtorIBAN = input.DebtorIBAN,
            Description = input.Description,
        };

        await transactionTableCollector.AddAsync(transaction.ToTable());

        return new OkObjectResult(transaction);
    }
}