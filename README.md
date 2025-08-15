# Azurite Demo: Local Azure Development Made Easy

A complete demonstration of local Azure development using Azurite, Azure Queues, and Azure Functions - all running locally without requiring an Azure subscription!

## 🎯 What This Project Demonstrates

This project shows how to:
- Develop Azure applications locally using Azurite (Azure Storage Emulator)
- Create a queue-based messaging system with Azure Storage Queues
- Build Azure Functions that process queue messages
- Seamlessly transition from local development to Azure cloud **without changing any code**

## 🏗️ Architecture Overview

```
[QueuesDemo Console App] → [Azure Queue (Azurite)] → [Azure Function]
       Produces messages         Stores messages        Processes messages
```

## 📁 Project Structure

```
AzuriteDemo/
├── QueuesDemo/              # Console app that sends messages to queue
│   ├── Program.cs           # Main application logic
│   ├── appsettings.json     # Local configuration
│   └── appsettings.Production.json  # Production configuration
├── FunctionDemo/            # Azure Function that processes queue messages
│   ├── TaskProcessor.cs     # Queue trigger function
│   ├── local.settings.json  # Local function settings
│   └── host.json           # Function host configuration
├── azurite/                # Azurite storage data (auto-generated)
└── README.md               # This file
```

## ✅ Prerequisites

Before getting started, ensure you have the following installed:

### Required Software

1. **Visual Studio Code** or **Visual Studio 2022**
   - [Download VS Code](https://code.visualstudio.com/)
   - [Download Visual Studio](https://visualstudio.microsoft.com/)

2. **.NET 8 SDK**
   - [Download from Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Verify installation: `dotnet --version`

3. **Node.js** (Required for Azure Functions Core Tools and Azurite)
   - [Download Node.js](https://nodejs.org/) (LTS version recommended)
   - Verify installation: `node --version`

4. **Azure Functions Core Tools v4**
   ```bash
   npm install -g azure-functions-core-tools@4 --unsafe-perm true
   ```
   - Verify installation: `func --version`

5. **Azurite** (Azure Storage Emulator)
   ```bash
   npm install -g azurite
   ```
   - Verify installation: `azurite --version`

### Optional but Recommended

- **Azure Storage Explorer** - GUI tool for viewing and managing storage
  - [Download Azure Storage Explorer](https://azure.microsoft.com/features/storage-explorer/)

## 🚀 Getting Started

### Step 1: Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/AzuriteDemo.git
cd AzuriteDemo
```

### Step 2: Start Azurite (Local Azure Storage)

Open a terminal in the project root and run:

```bash
azurite --silent --location ./azurite --debug ./azurite/debug.log
```

**Keep this terminal open** - Azurite needs to run continuously during development.

### Step 3: Build the Projects

```bash
# Build the console application
cd QueuesDemo
dotnet restore
dotnet build

# Build the Azure Function
cd ../FunctionDemo
dotnet restore
dotnet build
```

### Step 4: Start the Azure Function

In a new terminal, navigate to the FunctionDemo folder and start the function:

```bash
cd FunctionDemo
func start
```

You should see output similar to:
```
Azure Functions Core Tools
Core Tools Version: 4.x.x
Function Runtime Version: 4.x.x

Functions:
    TaskProcessor: queueTrigger

Host started
```

### Step 5: Run the Queue Producer

In another terminal, run the console application:

```bash
cd QueuesDemo
dotnet run
```

## 🎮 Watch It Work!

1. The console app will send several task messages to the queue
2. The Azure Function will automatically pick up and process each message
3. You'll see logs in both terminals showing the message flow
4. Check Azure Storage Explorer to see the queue in action!

## 📊 Monitoring Your Application

### Using Azure Storage Explorer

1. Open Azure Storage Explorer
2. Connect to "Local & Attached" → "Storage Accounts" → "Emulator - Default Ports"
3. Navigate to "Queues" → "tasks"
4. Watch messages being added and processed in real-time!

### Function Logs

The Azure Function will display detailed logs showing:
- When messages are received
- Processing status
- Any errors that occur

## 🌐 Moving to Production

The beauty of this setup is that moving to Azure requires **zero code changes** - only configuration updates!

### For QueuesDemo (Console App)

Replace the connection string in `appsettings.Production.json`:

```json
{
  "AzureStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=YOUR_STORAGE_ACCOUNT;AccountKey=YOUR_KEY;EndpointSuffix=core.windows.net",
    "QueueName": "tasks"
  }
}
```

### For FunctionDemo (Azure Function)

Update the Application Settings in your Azure Function App:

- `AzureWebJobsStorage`: Your Azure Storage connection string
- `QueueName`: "tasks" (or your preferred queue name)

That's it! Your application will work identically in Azure.

## 🛠️ Troubleshooting

### Common Issues

**Azurite won't start**
- Error: Port already in use
- Solution: Use different ports:
  ```bash
  azurite --blobPort 10001 --queuePort 10002 --tablePort 10003
  ```

**Function can't connect to queue**
- Check that Azurite is running
- Verify `local.settings.json` has `"AzureWebJobsStorage": "UseDevelopmentStorage=true"`

**No messages being processed**
- Ensure queue names match in both applications
- Check that the Function is running and showing the TaskProcessor function

**Build errors**
- Run `dotnet restore` in both project folders
- Ensure .NET 8 SDK is installed

### Useful Commands

```bash
# Check if Azurite is running
netstat -an | findstr "10000"

# Reset Azurite data
# Stop Azurite, delete the azurite folder, restart Azurite

# View Function logs in detail
func start --verbose

# Build and run console app
dotnet run --project QueuesDemo
```

## 📚 Learning Resources

- [Azure Storage Queues Documentation](https://docs.microsoft.com/azure/storage/queues/)
- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions/)
- [Azurite Documentation](https://docs.microsoft.com/azure/storage/common/storage-use-azurite)
- [.NET Azure SDK Documentation](https://docs.microsoft.com/dotnet/azure/)

## 🤝 Contributing

Feel free to submit issues, feature requests, or pull requests to improve this demo!

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📝 Blog Post

Check out the detailed blog post explaining this project: [Building Azure Queue and Functions Locally with Azurite: A Beginner's Guide](https://your-blog-url.com)

---

**Happy coding!** 🎉

If you found this helpful, please ⭐ star this repository and share it with other developers!
