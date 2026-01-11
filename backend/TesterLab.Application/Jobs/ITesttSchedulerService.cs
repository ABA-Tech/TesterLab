using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesterLab.Application.Jobs
{
    public interface ITestSchedulerService
    {
        Task ExecuteAsync(int jobId);
    }
}