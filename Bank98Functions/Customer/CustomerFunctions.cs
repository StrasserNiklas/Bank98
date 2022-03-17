using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bank98Functions.Customer;

public class CustomerFunctions
{
    [FunctionName("CreateCustomer")]
    public static async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customer")] HttpRequest req,
        [Table("Customers", Connection = "AzureWebJobsStorage")] IAsyncCollector<CustomerEntity> customerTableCollector,
        ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<CustomerDto>(requestBody);

        if (input.Name == null || input.IBAN == null || input.Address == null)
        {
            return new BadRequestObjectResult("Please provide all neccessary information.");
        }

        var customer = new Customer
        {
            Address = input.Address,
            Name = input.Name,
            IBAN = input.IBAN
        };

        await customerTableCollector.AddAsync(customer.ToTable());

        return new OkObjectResult(customer);
    }

    [FunctionName("GetAllCustomers")]
    public static async Task<IActionResult> GetAllCustomers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customer/all")] HttpRequest req,
        [Table("customers", Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
        ILogger log)
    {
        TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>();

        var segment = await cloudTable.ExecuteQuerySegmentedAsync(query, null);
        var data = segment.Select(CustomerExtensions.ToCustomer);

        return new OkObjectResult(data);
    }

    [FunctionName("GetCustomer")]
    public static async Task<IActionResult> GetCustomer(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customer")] HttpRequest req,
        [Table("customers", Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
        ILogger log)
    {
        string id = req.Query["id"];

        var operation = TableOperation.Retrieve<CustomerEntity>("CUSTOMER", id);
        var customer = await cloudTable.ExecuteAsync(operation);

        return new OkObjectResult(customer.Result);
    }
}
