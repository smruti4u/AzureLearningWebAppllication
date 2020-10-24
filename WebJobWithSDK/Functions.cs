using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebJobWithSDK
{
    public class Functions
    {
        public static void ProcessMessage([QueueTrigger("iqueue")] Order order, ILogger logger)
        {
            logger.LogInformation("Message Arrived");
        }

        public static void TimerTrigger([TimerTrigger("0 */2 * * * *", RunOnStartup = true)] TimerInfo timerInfo, ILogger logger)
        {
            logger.LogInformation($"Timer Trigger Triggered {DateTime.Now}");
        }
    }

    public class Order
    {
        public string Id { get; set; }
        public string Price { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Pin { get; set; }
        public string City { get; set; }
    }
}
