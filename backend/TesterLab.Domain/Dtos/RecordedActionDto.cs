namespace TesterLab.Domain.DTOs
{
    public class RecordedActionDto
    {
        public string? Selector { get; set; }
        public long Timestamp { get; set; }
        public long CapturedAt { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }
        public string? ExpectedValue { get; set; }
        public string? ActualValue { get; set; }
        public string? Xpath { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public string? TagName { get; set; }
        public int SequenceNumber { get; set; }
        public string? EventId { get; set; }
        public RecordedLocatorsDto? Locators { get; set; }
        public RecordedPageDto? Page { get; set; }
    }

    public class RecordedLocatorsDto
    {
        public string? Css { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Xpath { get; set; }
        public string? Text { get; set; }
    }

    public class RecordedPageDto
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
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
        public string Text { get; set; }
        public bool IsOptional { get; set; }
        public string? Selector { get; set; }
        public string? TagName { get; set; }
    }

    public class TestStepImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int ImportedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    // Classes pour les données de graphiques
    public class DailyTrendData
    {
        public DateTime Date { get; set; }
        public int TotalRuns { get; set; }
        public int PassedRuns { get; set; }
        public int FailedRuns { get; set; }
        public double SuccessRate { get; set; }
    }

    public class ChartDataPoint
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string Label { get; set; }
    }
}