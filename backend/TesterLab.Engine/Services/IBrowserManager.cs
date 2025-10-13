using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TesterLab.Engine.Models;

namespace TesterLab.Engine.Services
{
    public interface IBrowserManager
    {
        Task InitializeAsync(BrowserConfiguration configuration);
        Task NavigateToAsync(string url);
        Task<IWebElementWrapper> FindElementAsync(string element, int? timeoutOverride = null);
        Task<List<IWebElementWrapper>> FindElementsAsync(string element, int? timeoutOverride = null);
        Task<byte[]> TakeScreenshotAsync();
        Task CloseAsync();
        Task DisposeAsync();
        bool IsInitialized { get; }
    }
}
