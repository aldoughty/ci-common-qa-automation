using Destructurama.Attributed;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace ci_common_qa_automation.BaseObjects
{
   
    public class TestResults
    {
        private readonly Serilog.Core.Logger TestResultLogger;

        [JsonProperty(PropertyName = "BatchId")]
        public string BatchId { get; set; }
        [JsonProperty(PropertyName = "TenantName")]
        public string TenantName { get; set; }
        [JsonProperty(PropertyName = "ApplicationUnderTest")]
        public string ApplicationUnderTest { get; set; }
        [JsonProperty(PropertyName = "AutomationMicroService")]
        public string AutomationMicroService { get; set; }
        [JsonProperty(PropertyName = "Environment")]
        public string Environment { get; set; }
        [JsonProperty(PropertyName = "NumberOfTestsCreated")]
        public int NumberOfTestsCreated { get; set; }
        [JsonProperty(PropertyName = "NumberTestsExecuted")]
        public int NumberTestsExecuted { get; set; }
        [JsonProperty(PropertyName = "NumberTestsPassed")]
        public int NumberTestsPassed { get; set; }
        [JsonProperty(PropertyName = "NumberTestsFailed")]
        public int NumberTestsFailed { get; set; }
        [JsonProperty(PropertyName = "NumberTestsWithWarning")]
        public int NumberTestsWithWarning { get; set; }
        [JsonProperty(PropertyName = "NumberTestsWithError")]
        public int NumberTestsWithError { get; set; }
        [JsonProperty(PropertyName = "SecondsToExecute")]
        public int SecondsToExecute { get; set; }
        [JsonProperty(PropertyName = "ExecutionDate")]
        public string ExecutionDate { get; set; }
        [JsonProperty(PropertyName = "IndividualResults")]
        [NotLogged]
        internal List<TestDetails> IndividualResults { get; set; }
        public class TestDetails
        {
            [JsonProperty(PropertyName = "TestCaseName")]
            public string? TestCaseName { get; set; }
            [JsonProperty(PropertyName = "TestCaseStep")]
            public string? TestCaseStep { get; set; }
            [JsonProperty(PropertyName = "Status")]
            public Status Status { get; set; }
            [JsonProperty(PropertyName = "FailureMessage")]
            public string? FailureMessage{ get; set; }
            [JsonProperty(PropertyName = "ExecutionDate")]
            public string? ExecutionDate{ get; set; }
            [JsonProperty(PropertyName = "MilliSecondsToExecute")]
            public long MilliSecondsToExecute { get; set; }
        }
        public TestResults(StandardTestParameters parameters)
        {
            BatchId = parameters.BatchId;
            TenantName = parameters.TenantName;
            ApplicationUnderTest = parameters.ApplicationUnderTest;
            AutomationMicroService = parameters.AutomationMiroService;
            Environment = parameters.Environment;
            TestResultLogger = Utilities.Logger.CreateTestCaseLogger(parameters);
            ExecutionDate = $"{DateAndTime.Now.ToString():MM-dd-yyyy HH:mm:ss}";
            IndividualResults = new List<TestDetails>();
        }
        public void LogIndividualResults(TestDetails testResult)
        {
            TestResultLogger.Information("Message Parameters: {@Data}", testResult);
            IndividualResults.Add(testResult);
        }
        public void LogFinalResults()
        {
            NumberTestsExecuted = IndividualResults.Count;
            NumberTestsPassed = IndividualResults.Where(x => x.Status == Status.Passed).ToList().Count;
            NumberTestsFailed = IndividualResults.Where(x => x.Status == Status.Failed).ToList().Count;
            NumberTestsWithWarning = IndividualResults.Where(x => x.Status == Status.Warning).ToList().Count;
            NumberTestsWithError = IndividualResults.Where(x => x.Status == Status.Error).ToList().Count;
            TestResultLogger.Information("Test Results for {@BatchId}: {@Data}",BatchId,this);
        }
        public void SetNumberOfTestsCreated(int numberOfTests)
        {
            NumberOfTestsCreated = numberOfTests;
        }
        public int GetNumberOfIndividualTests()
        {
            return IndividualResults.Count;
        }
        public enum Status
        {
            Passed,
            Failed,
            Warning,
            Error,
            Running
        }
    }
}
