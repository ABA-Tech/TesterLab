using System.Threading.Tasks;

namespace TesterLab.Engine.Models
{
    public class TestExecutionResult
    {
        public int TestCaseId { get; set; }
        public string TestCaseName { get; set; } = string.Empty;
        public TestStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration => EndTime - StartTime;
        public string? ErrorMessage { get; set; }
        public Exception? Exception { get; set; }
        public List<ActionResult> StepResults { get; set; } = new();
        public List<string> Screenshots { get; set; } = new();
        public Dictionary<string, object> AdditionalData { get; set; } = new();
    }

    public class ActionResult
    {
        public int StepId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Element { get; set; } = string.Empty;
        public string? Value { get; set; }
        public ActionStatus Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public TimeSpan Duration { get; set; }
        public string? ErrorMessage { get; set; }
        public Exception? Exception { get; set; }
        public string? Screenshot { get; set; }
    }

    public enum BrowserType
    {
        Chrome,
        Firefox,
        Edge,
        Safari
    }

    public enum TestStatus
    {
        NotStarted,
        Running,
        Passed,
        Failed,
        Skipped,
        Cancelled
    }

    public enum ActionStatus
    {
        Success,
        Failed,
        Skipped,
        Warning
    }

    public enum SelectorType
    {
        Id,
        Name,
        ClassName,
        CssSelector,
        XPath,
        TagName,
        LinkText,
        PartialLinkText
    }

    public enum ElementType
    {
        Button,
        Input,
        Select,
        Checkbox,
        Radio,
        Link,
        Text,
        Image,
        Table,
        Div,
        Span,
        Generic
    }
}
