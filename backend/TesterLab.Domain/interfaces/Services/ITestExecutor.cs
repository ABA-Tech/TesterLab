using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.Models;

namespace TesterLab.Domain.interfaces.Services
{
    public interface ITestExecutor
    {
        Task<TestExecutionResult> ExecuteTestCaseAsync(TestCase testCase, TestRun testRun);
        Task<StepResult> ExecuteTestStepAsync(TestStep testStep, IWebDriver driver, Dictionary<string, string> testData);
        Task<bool> ValidateStepAsync(TestStep testStep);
        Task<byte[]> TakeScreenshotAsync(IWebDriver driver);
    }

    public class TestExecutionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
        public List<StepResult> StepResults { get; set; } = new();
        public List<string> Screenshots { get; set; } = new();
        public string ErrorDetails { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class StepResult
    {
        public int StepId { get; set; }
        public int Order { get; set; }
        public string Action { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public TimeSpan Duration { get; set; }
        public string Screenshot { get; set; }
        public string ErrorMessage { get; set; }
    }
}
