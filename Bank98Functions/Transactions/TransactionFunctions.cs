using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<TransactionDto>(requestBody);

        if (string.IsNullOrWhiteSpace(input.CreditorIBAN) || string.IsNullOrWhiteSpace(input.DebtorIBAN) || input.Amount == 0)
        {
            return new BadRequestObjectResult("Please provide all neccessary information.");
        }

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