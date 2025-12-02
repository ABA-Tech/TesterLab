
namespace TesterLab.Domain.DTOs
{
    public class RecordedActionDto
    {
        public string Selector { get; set; }
        public long Timestamp { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Xpath { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }

    public class TestStepImportDto
    {
        public int Order { get; set; }
        public string Action { get; set; }
        public string Target { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Xpath { get; set; }
        public int TimeoutSeconds { get; set; }
        public string Text {get;set;}
        public bool IsOptional { get; set; }
        public string? Selector { get; set; }
    }

    public class TestStepImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ImportedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}