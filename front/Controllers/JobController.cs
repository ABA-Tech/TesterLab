using Microsoft.AspNetCore.Mvc;
using TesterLab.Domain.interfaces.Services;
using TesterLab.Domain.Models;
using TesterLab.JobScheduler.Dtos;
using TesterLab.JobScheduler.Services;

namespace TesterLab.Controllers
{
  public class JobController : Controller
  {
    private readonly JobRepository _jobRepository;
    private readonly ITestCaseService _testCaseService;
    public JobController(JobRepository jobRepository, ITestCaseService testCaseService)
    {
      _jobRepository = jobRepository;
      _testCaseService = testCaseService;
    }

    // GET: JobController
    public ActionResult Index()
    {
      return View();
    }

    // GET: JobController/Details/5
    public ActionResult Details(int id)
    {
      return View();
    }


    // GET: JobController/Details/5
    public async Task<IActionResult> GetJobByIdJson(int id)
    {
      try
      {
        var job = await _jobRepository.GetJob(id);
        return Json(job);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    // GET: JobController/Create
    [HttpPost]
    public async Task<IActionResult> Create(CreateJobRequest request)
    {
      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(request.TestCaseId);
      if(testCase == null) return NotFound();

      var job = new Job
      {
        Name = "Job -" + testCase.Name,
        Description = testCase.Description,
        IsRunning = false,
        IsEnabled = true,
        NextExecutionTimeUtc = request.FirstExecutionTimeUtc,  
        FrequencyInMinutes = request.FrequencyInMinutes,
        EnvironmentId = request.EnvironmentId,
        TestCaseId = request.TestCaseId,
        CreatedAtUtc = DateTime.Now,
        UpdatedAtUtc = DateTime.Now
      };

      var res = await _jobRepository.AddJob(job);
      return Ok(res);
    }

    // GET: JobController/Create
    [HttpPost]
    public async Task<IActionResult> Update(int id, UpdateJobRequest request)
    {
      var testCase = await _testCaseService.GetTestCaseWithStepsAsync(request.TestCaseId);
      if (testCase == null) return NotFound();

      var job = await _jobRepository.GetJob(id);

      if(job == null) return NotFound();

      job.NextExecutionTimeUtc = request.NextExecutionTimeUtc;
      job.EnvironmentId = request.EnvironmentId;
      job.FrequencyInMinutes = request.FrequencyInMinutes;
      job.IsEnabled = request.IsEnabled;
      job.UpdatedAtUtc = DateTime.Now;

      var res = await _jobRepository.UpdateJob(job);
      return Ok(res);
    }

    //// POST: JobController/Create
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult Create(IFormCollection collection)
    //{
    //  try
    //  {
    //    return RedirectToAction(nameof(Index));
    //  }
    //  catch
    //  {
    //    return View();
    //  }
    //}

    // GET: JobController/Edit/5
    public ActionResult Edit(int id)
    {
      return View();
    }

    // POST: JobController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, IFormCollection collection)
    {
      try
      {
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }

    // GET: JobController/Delete/5
    public ActionResult Delete(int id)
    {
      return View();
    }

    // POST: JobController/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id, IFormCollection collection)
    {
      try
      {
        return RedirectToAction(nameof(Index));
      }
      catch
      {
        return View();
      }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteJob([FromBody] DeleteJobDto dto)
    {
      var job = await _jobRepository.GetJob(dto.Id);
      if(job == null)
        return NotFound();

      await _jobRepository.DeleteJob(job);
      return Ok(job);
    }
  }

  public class DeleteJobDto
  {
    public int Id { get; set; }
  }
}
