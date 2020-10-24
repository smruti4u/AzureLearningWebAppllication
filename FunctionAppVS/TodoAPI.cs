using FunctionAppVS.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionAppVS
{
    public static class TodoAPI
    {
        static List<ToDoItem> items = new List<ToDoItem>();

        [FunctionName("CreteTask")]
        public static async Task<IActionResult> CreteTask(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todo")] HttpRequest req,
    ILogger log)
        {
            string reqBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<CreateViewModel>(reqBody);

            if(input == null || string.IsNullOrWhiteSpace(input.Description))
            {
                return new BadRequestResult();
            }

            ToDoItem newItem = new ToDoItem(input.Description);
            newItem.Id = Guid.NewGuid().ToString();
            newItem.IsCompleted = false;
            items.Add(newItem);

            return new OkObjectResult(items);
        }

        [FunctionName("UpdateTask")]
        public static async Task<IActionResult> UpdateTask(
[HttpTrigger(AuthorizationLevel.Function, "put", Route = "todo/{id}")] HttpRequest req,
ILogger log, string id)
        {
            var currentItem = items.Where(x => x.Id == id).FirstOrDefault();
            if(currentItem == null)
            {
                return new NotFoundObjectResult($"Item With Id {id} is not created");
            }

            currentItem.IsCompleted = true;
            return new OkObjectResult(currentItem);
        }

        [FunctionName("GetAllTask")]
        public static async Task<IActionResult> GetAllTasks(
[HttpTrigger(AuthorizationLevel.Function, "Get", Route = "todo")] HttpRequest req,
ILogger log)
        {
            var pendingTasks = items.Where(x => x.IsCompleted == false)?.ToList();
            return new OkObjectResult(pendingTasks);
        }
    }
}
