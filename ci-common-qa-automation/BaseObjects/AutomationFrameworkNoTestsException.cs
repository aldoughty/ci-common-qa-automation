namespace ci_common_qa_automation.BaseObjects
{
    public class AutomationFrameworkNoTestsException : Exception
    {
        public AutomationFrameworkNoTestsException()
        {
        }

        public AutomationFrameworkNoTestsException(string message) : base(message)
        {
        }

        public AutomationFrameworkNoTestsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
