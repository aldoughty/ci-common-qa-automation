namespace ci_common_qa_automation.BaseObjects
{
    public class AutomationFrameworkException : Exception
    {
        public string ActionTestName { get; set; }

        public AutomationFrameworkException()
        {
            ActionTestName = "ExceptionMessageBaseActionTestName";
        }

        public AutomationFrameworkException(string message) : base(message)
        {
            ActionTestName = "ExceptionMessageBaseActionTestName";
        }

        public AutomationFrameworkException(string message, Exception inner, string actionTestName) : base(message, inner)
        {
            ActionTestName = actionTestName;
        }

        public AutomationFrameworkException(string message, Exception inner) : base(message, inner)
        {
            ActionTestName = "ExceptionMessageBaseActionTestName";
        }
    }
}
