using System.Data;

namespace ci_common_qa_automation.BaseObjects
{
    public static class BatchTestStatus
    {
        public const string PASS = "Passed";
        public const string FAILED = "Failed";
        public const string RUNNING = "Running";
        public const string WARNING = "Warning";
    }
    public static class TestStatus
    {

        public static TestResults.Status GetTestStatus(string detailResults, bool isCritical)
        {
            if (string.IsNullOrEmpty(detailResults)) return TestResults.Status.Passed;
            return !isCritical ? TestResults.Status.Warning : TestResults.Status.Failed;
        }
        public static TestResults.Status GetTestStatus(object? detailResults, bool isCritical)
        {
            if (detailResults == null) return TestResults.Status.Passed;
            return !isCritical ? TestResults.Status.Warning : TestResults.Status.Failed;
        }
        public static TestResults.Status GetResultExpectedQueryWithNoResultsStatus(string detailResults, DataTable results, decimal threshold, bool isCritical)
        {
            if (string.IsNullOrEmpty(detailResults)) return TestResults.Status.Passed;
            if (!isCritical) return TestResults.Status.Warning;
            if (results.Columns.Contains("Result"))
            {
                if (results.Rows[0]["Result"].ToString().Contains("TOO LARGE")) return TestResults.Status.Failed;
            }
            int resultCount = results.Rows.Count;
            if (isCritical && resultCount <= threshold) return TestResults.Status.Warning;
            return TestResults.Status.Failed;

        }
    }
}
