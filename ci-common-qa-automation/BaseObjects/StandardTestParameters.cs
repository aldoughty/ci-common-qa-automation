namespace ci_common_qa_automation.BaseObjects
{
    public class StandardTestParameters
    {
        public string? BatchId { get; set; }
        public string? TenantName { get; set; }
        public string? ApplicationUnderTest { get; set; }
        public string? AutomationMiroService { get; set; }
        public string? Environment { get; set; }
        public bool LocalLogs { get; set; }
    }
}
