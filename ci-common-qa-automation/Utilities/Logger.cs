using Azure.Messaging.EventHubs.Producer;
using ci_common_qa_automation.BaseObjects;
using Destructurama;
using Elastic.CommonSchema.Serilog;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

namespace ci_common_qa_automation.Utilities
{
    public static class Logger 
    {
        public static Serilog.Core.Logger CreateSystemLogger(StandardTestParameters stp)
        {
            EventHubProducerClient client = new("Endpoint=sb://ehn-sharedprod-monitoring.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yVNZ40BP1qZm0mTfZSjMYYuVDAbEwi7/Ht6qog1v7l0=", "eh-rxslogging");
            if (stp.LocalLogs)
            {
                return new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.WithProperty("Framework", "AutomationFramework")
                    .Enrich.WithProperty("ApplicationUnderTest", stp.ApplicationUnderTest)
                    .Enrich.WithProperty("AutomationMiroService", stp.AutomationMiroService)
                    .Enrich.WithProperty("Environment", stp.Environment)
                    .Enrich.WithProperty("BatchId", stp.BatchId)
                    //.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .CreateLogger();
            }
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("Framework", "AutomationFramework")
                .Enrich.WithProperty("ApplicationUnderTest", stp.ApplicationUnderTest)
                .Enrich.WithProperty("AutomationMiroService", stp.AutomationMiroService)
                .Enrich.WithProperty("Environment", stp.Environment)
                .Enrich.WithProperty("BatchId", stp.BatchId)
                .Enrich.WithCorrelationId()
                .Enrich.WithExceptionDetails()
                .WriteTo.AzureEventHub(new EcsTextFormatter(), client, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .WriteTo.Console()
                .CreateLogger();
        }
        public static Serilog.Core.Logger CreateTestCaseLogger(StandardTestParameters stp)
        {
            EventHubProducerClient client = new("Endpoint=sb://ehn-sharedprod-monitoring.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yVNZ40BP1qZm0mTfZSjMYYuVDAbEwi7/Ht6qog1v7l0=", "eh-rxslogging");
            if (stp.LocalLogs)
            {
                return new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.WithProperty("Framework", "AutomationFramework")
                    .Enrich.WithProperty("ApplicationUnderTest", stp.ApplicationUnderTest)
                    .Enrich.WithProperty("AutomationMiroService", stp.AutomationMiroService)
                    .Enrich.WithProperty("Environment", stp.Environment)
                    .Enrich.WithProperty("BatchId", stp.BatchId)
                    .Destructure.UsingAttributes()
                    //.WriteTo.Console(new JsonFormatter())
                    .WriteTo.File(new JsonFormatter(), "TestResultLog.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("Framework", "AutomationFramework")
                .Enrich.WithProperty("ApplicationUnderTest", stp.ApplicationUnderTest)
                .Enrich.WithProperty("AutomationMiroService", stp.AutomationMiroService)
                .Enrich.WithProperty("Environment", stp.Environment)
                .Enrich.WithProperty("BatchId", stp.BatchId)
                .Enrich.WithCorrelationId()
                .Enrich.WithExceptionDetails()
                .Destructure.UsingAttributes()
                //Need to keep TextFormatter as EscTextFormatter since this is what EventGrid uses.  We need to investigate if we can use
                //JsonFormatter so that we can have Failure Message as an object instead of a string
                .WriteTo.AzureEventHub(new EcsTextFormatter(), client, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                //.WriteTo.Console(new JsonFormatter())
                .CreateLogger();
        }
        public static Serilog.Core.Logger CreateSystemLoggerWithConfigurationFile(string filePath, string fileName)
        {
            IConfigurationRoot? configuration = new ConfigurationBuilder()
                                                .SetBasePath(filePath)
                                                .AddJsonFile(fileName)
                                                .AddEnvironmentVariables()
                                                .Build();
            return new  LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .CreateLogger();
        }
    }
}
