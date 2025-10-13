using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.Models;
using TesterLab.Engine.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Engine.Services
{
    public interface ITestExecutor 
    {
        Task<TestExecutionResult> ExecuteTestCaseAsync(TestCase testCase, Environment environment, DataSet? dataSet = null);
        Task<TestExecutionResult> ExecuteTestCaseAsync(TestCaseDto testCase, Environment environment, DataSet? dataSet = null);
        Task<List<TestExecutionResult>> ExecuteEnvironmentAsync(Environment environment, DataSet? dataSet = null);
        Task StopExecutionAsync();
    }
}
