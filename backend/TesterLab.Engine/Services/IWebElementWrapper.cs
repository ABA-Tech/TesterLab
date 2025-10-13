using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TesterLab.Engine.Services
{

    public interface IWebElementWrapper
    {
        Task ClickAsync();
        Task SendKeysAsync(string text);
        Task ClearAsync();
        Task<string> GetTextAsync();
        Task<string> GetAttributeAsync(string attributeName);
        Task<bool> IsDisplayedAsync();
        Task<bool> IsEnabledAsync();
        Task<bool> IsSelectedAsync();
        Task WaitForVisibilityAsync(int timeoutSeconds = 30);
        Task WaitForClickabilityAsync(int timeoutSeconds = 30);
        Task ScrollIntoViewAsync();
        //Element ElementMetadata { get; }
    }
}
