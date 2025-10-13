using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Engine.Models;

namespace TesterLab.Engine.Services
{
    public interface ITestLogger
    {
        void LogInfo(string message, TestExecution? execution = null);
        void LogWarning(string message, TestExecution? execution = null);
        void LogError(string message, Exception? exception = null, TestExecution? execution = null);
        void LogAction(string action, string element, string value, TestExecution? execution = null);
        Task FlushAsync();
    }
}
