using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionAppVS
{
    public static class DurableOrcheStration
    {
        [FunctionName("DurableOrcheStration")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            var tsk1 =  context.CallActivityAsync<string>("DurableOrcheStration_Hello", "Tokyo");
            var task2 = context.CallActivityAsync<string>("DurableOrcheStration_Hello", "Mumbai");
            var task3 =  context.CallActivityAsync<string>("DurableOrcheStration_Hello", "London");

            await Task.WhenAll(tsk1, task2, task3);
            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [FunctionName("DurableOrcheStration_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("DurableOrcheStration_Hello2")]
        public static string SayHello2([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name} + from hello2.");
            return $"Hello {name}!";
        }

        [FunctionName("DurableOrcheStration_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("DurableOrcheStration", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}