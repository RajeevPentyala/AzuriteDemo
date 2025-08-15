using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionDemo
{
    public class TaskProcessor
    {
        [FunctionName("TaskProcessor")]
        public void Run([QueueTrigger("%QueueName%", Connection = "AzureWebJobsStorage")]string queueMessage, ILogger log)
        {
            try
            {
                log.LogInformation("🚀 Azure Function triggered! Processing task...");
                log.LogInformation($"📨 Task received: {queueMessage}");
                
                // Simulate processing work
                System.Threading.Thread.Sleep(2000);
                
                log.LogInformation($"✅ Task completed successfully: {queueMessage}");
            }
            catch (Exception ex)
            {
                log.LogError($"❌ Error processing task: {ex.Message}");
                throw; // Re-throw to let Azure Functions handle retry logic
            }
        }
    }
}
