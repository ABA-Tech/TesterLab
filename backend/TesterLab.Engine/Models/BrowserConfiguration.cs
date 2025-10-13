using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterLab.Engine.Models
{
    public class BrowserConfiguration
    {
        public BrowserType BrowserType { get; set; } = BrowserType.Chrome;
        public bool Headless { get; set; } = false;
        public TimeSpan ImplicitWait { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan PageLoadTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan ScriptTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public string? DownloadDirectory { get; set; }
        public List<string> Arguments { get; set; } = new();
        public Dictionary<string, object> Preferences { get; set; } = new();
    }
}
