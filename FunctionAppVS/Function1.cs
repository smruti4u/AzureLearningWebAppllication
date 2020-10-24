using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionAppVS
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }


        [FunctionName("TimerTrigger")]
        public static async Task TimerTrigger(
    [TimerTrigger("0 */2 * * * *", RunOnStartup = true)] TimerInfo timerInfo,
    ILogger log)
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetAsync("https://azurefunctlearn.azurewebsites.net/api/todo?code=sxN3waimwHBfnaq3jWNG0trVAmnRgnszzQAU6SOARS7d/n566AH/JA==");
            var content = result.Content.ReadAsStringAsync().Result;
        }

        [FunctionName("IOBinding")]
        [return : Table("DTOTable")]
        public static async Task<MyDTO> IOBinding(
[TimerTrigger("0 */2 * * * *", RunOnStartup = true)] TimerInfo timerInfo,
[Blob("blobs/input.txt", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream blob,
ILogger log)
        {
            StreamReader reader = new StreamReader(blob);
            JObject jObject = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            MyDTO dto = new MyDTO()
            {
                text = jObject
            };
            return dto;
        }
    }

    public class MyDTO : TableEntity
    {
        public MyDTO()
        {
            this.PartitionKey = Guid.NewGuid().ToString();
            this.RowKey = "12345";
        }

        public JObject text { get; set; }
    }
}
