// See https://aka.ms/new-console-template for more information
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;

Console.WriteLine("=== Azure Queue Storage Demo with Azurite ===");
Console.WriteLine();

// Load configuration
var configuration = LoadConfiguration();

// Step 1: Test connection to queue
var queueClient = await ConnectToQueue(configuration);
if (queueClient != null)
{
    Console.WriteLine("✅ Successfully connected to queue!");
    
    // Step 2: Push messages to queue
    await PushMessagesToQueue(queueClient);
    
    Console.WriteLine();
    Console.WriteLine("⏳ Messages sent! Check the Azure Function logs to see them being processed...");
    Console.WriteLine("💡 The Azure Function will automatically process these messages!");
    
    // Step 3: Reading logic commented out so Azure Function can process the messages
    // await ReadMessagesFromQueue(queueClient);
}
else
{
    Console.WriteLine("❌ Failed to connect to queue.");
}

/// <summary>
/// Loads configuration from appsettings.json
/// </summary>
/// <returns>IConfiguration object</returns>
static IConfiguration LoadConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
}

/// <summary>
/// Establishes connection to the queue using configuration
/// </summary>
/// <param name="configuration">Configuration object</param>
/// <returns>QueueClient if successful, null if failed</returns>
static async Task<QueueClient?> ConnectToQueue(IConfiguration configuration)
{
    try
    {
        Console.WriteLine("🔄 Connecting to Azure queue storage...");
        
        // Read connection string and queue name from configuration
        string connectionString = configuration["AzureStorage:ConnectionString"] 
            ?? throw new InvalidOperationException("Connection string not found in configuration");
        string queueName = configuration["AzureStorage:QueueName"] 
            ?? throw new InvalidOperationException("Queue name not found in configuration");
        
        Console.WriteLine($"📡 Using connection: {(connectionString.Contains("UseDevelopmentStorage") ? "Azurite (Local)" : "Azure Storage (Cloud)")}");
        
        // Create QueueClient with plain text encoding to avoid double Base64 encoding
        var queueClientOptions = new QueueClientOptions();
        queueClientOptions.MessageEncoding = QueueMessageEncoding.None;
        var queueClient = new QueueClient(connectionString, queueName, queueClientOptions);
        
        // Verify the queue exists (this will also test the connection)
        var response = await queueClient.ExistsAsync();
        
        if (response.Value)
        {
            Console.WriteLine($"📋 Connected to queue: '{queueName}'");
            return queueClient;
        }
        else
        {
            Console.WriteLine($"⚠️  Queue '{queueName}' does not exist.");
            return null;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Connection failed: {ex.Message}");
        return null;
    }
}

/// <summary>
/// Reads and processes messages from the queue
/// </summary>
/// <param name="queueClient">The queue client to use</param>
static async Task ReadMessagesFromQueue(QueueClient queueClient)
{
    try
    {
        Console.WriteLine();
        Console.WriteLine("📥 Reading messages from queue...");
        
        int messageCount = 0;
        const int maxMessages = 10; // Safety limit to avoid infinite loop
        
        // Keep reading messages until queue is empty or we hit the limit
        while (messageCount < maxMessages)
        {
            // Receive a message (this makes it invisible to other consumers for 30 seconds by default)
            var response = await queueClient.ReceiveMessageAsync();
            
            if (response.Value == null)
            {
                // No more messages in the queue
                Console.WriteLine("📭 No more messages in the queue.");
                break;
            }
            
            var message = response.Value;
            messageCount++;
            
            // Display the message
            Console.WriteLine($"📨 [{messageCount}] Processing: {message.MessageText}");
            
            // Simulate processing work
            Console.WriteLine($"⏳ Processing task...");
            await Task.Delay(1000); // Simulate work being done
            
            // Delete the message after successful processing
            await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            Console.WriteLine($"✅ [{messageCount}] Task completed and removed from queue");
            
            Console.WriteLine(); // Empty line for readability
        }
        
        Console.WriteLine($"🎉 Finished processing {messageCount} messages!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to read messages: {ex.Message}");
    }
}

/// <summary>
/// Adds sample task messages to the queue
/// </summary>
/// <param name="queueClient">The queue client to use</param>
static async Task PushMessagesToQueue(QueueClient queueClient)
{
    try
    {
        Console.WriteLine();
        Console.WriteLine("📤 Pushing messages to queue...");
        
        // Sample task messages to add to the queue
        string[] taskMessages = {
            "Send welcome email to new user",
            "Process payment for order #12345", 
            "Generate monthly sales report",
            "Backup database at midnight"
        };
        
        // Push each message to the queue
        for (int i = 0; i < taskMessages.Length; i++)
        {
            await queueClient.SendMessageAsync(taskMessages[i]);
            Console.WriteLine($"✉️  [{i + 1}] Added: {taskMessages[i]}");
            
            // Small delay to make the demo more visible
            await Task.Delay(500);
        }
        
        Console.WriteLine($"✅ Successfully pushed {taskMessages.Length} messages to the queue!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to push messages: {ex.Message}");
    }
}
