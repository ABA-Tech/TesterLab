using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.Models;

namespace TesterLab.Engine.Services
{
    public interface IScreenshotManager
    {
        //Task<string> CaptureScreenshotAsync(IBrowserManager browserManager, TestExecution execution, TestStep step, string description = "");
        Task SaveScreenshotAsync(byte[] imageData, string filePath);
        //string GenerateScreenshotPath(TestExecution execution, TestStep step);
    }
}
