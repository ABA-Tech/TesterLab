using Microsoft.EntityFrameworkCore;
using TesterLab.Domain.DTOs;
using TesterLab.Domain.interfaces.Repositories;
using TesterLab.Domain.Models;

namespace TesterLab.Infrastructure.Data.Repositories
{
    public class TestRunRepository2 : ITestRunRepository2
    {
        private readonly TesterLabDbContext _context;

        public TestRunRepository2(TesterLabDbContext context)
        {
            _context = context;
        }

        public async Task<List<TestRun>> GetRecentAsync(int days)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            
            return await _context.TestRuns
                .Include(tr => tr.Application)
                .Include(tr => tr.Environment)
                .Where(tr => tr.CreatedAt >= startDate)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TestRun>> GetLatestAsync(int count)
        {
            return await _context.TestRuns
                .Include(tr => tr.Application)
                .Include(tr => tr.Environment)
                .OrderByDescending(tr => tr.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> CountRecentAsync(int days)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            
            return await _context.TestRuns
                .Where(tr => tr.CreatedAt >= startDate)
                .CountAsync();
        }

        public async Task<int> CountRecentByStatusAsync(int days, string status)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            
            // Pour le statut "Passed", on compte les runs complétés sans échecs
            if (status == "Passed")
            {
                return await _context.TestRuns
                    .Where(tr => tr.CreatedAt >= startDate 
                        && tr.Status == "Completed" 
                        && tr.FailedCount == 0)
                    .CountAsync();
            }
            
            // Pour le statut "Failed", on compte les runs avec des échecs
            if (status == "Failed")
            {
                return await _context.TestRuns
                    .Where(tr => tr.CreatedAt >= startDate 
                        && (tr.Status == "Failed" || tr.FailedCount > 0))
                    .CountAsync();
            }
            
            return await _context.TestRuns
                .Where(tr => tr.CreatedAt >= startDate && tr.Status == status)
                .CountAsync();
        }

        public async Task<double> GetSuccessRateAsync(int days)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);
            
            var totalRuns = await _context.TestRuns
                .Where(tr => tr.CreatedAt >= startDate && tr.Status == "Completed")
                .CountAsync();
            
            if (totalRuns == 0)
                return 0;
            
            var successfulRuns = await _context.TestRuns
                .Where(tr => tr.CreatedAt >= startDate 
                    && tr.Status == "Completed" 
                    && tr.FailedCount == 0)
                .CountAsync();
            
            return Math.Round((double)successfulRuns / totalRuns * 100, 2);
        }

        public async Task<List<DailyTrendData>> GetDailyTrendsAsync(int days)
        {
            var startDate = DateTime.UtcNow.AddDays(-days).Date;
            
            var runs = await _context.TestRuns
                .Where(tr => tr.CreatedAt >= startDate)
                .Select(tr => new 
                { 
                    Date = tr.CreatedAt.Date,
                    tr.Status,
                    tr.FailedCount,
                    tr.PassedCount
                })
                .ToListAsync();
            
            // Grouper par jour
            var dailyData = runs
                .GroupBy(r => r.Date)
                .Select(g => new DailyTrendData
                {
                    Date = g.Key,
                    TotalRuns = g.Count(),
                    PassedRuns = g.Count(r => r.Status == "Completed" && r.FailedCount == 0),
                    FailedRuns = g.Count(r => r.Status == "Failed" || r.FailedCount > 0),
                    SuccessRate = g.Count() > 0 
                        ? Math.Round((double)g.Count(r => r.Status == "Completed" && r.FailedCount == 0) / g.Count() * 100, 2)
                        : 0
                })
                .OrderBy(d => d.Date)
                .ToList();
            
            // Remplir les jours manquants avec des zéros
            var result = new List<DailyTrendData>();
            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var existingData = dailyData.FirstOrDefault(d => d.Date == date);
                
                if (existingData != null)
                {
                    result.Add(existingData);
                }
                else
                {
                    result.Add(new DailyTrendData
                    {
                        Date = date,
                        TotalRuns = 0,
                        PassedRuns = 0,
                        FailedRuns = 0,
                        SuccessRate = 0
                    });
                }
            }
            
            return result;
        }

        public async Task<List<ChartDataPoint>> GetSuccessRateTrendAsync(int days)
        {
            var dailyTrends = await GetDailyTrendsAsync(days);
            
            return dailyTrends.Select(dt => new ChartDataPoint
            {
                Date = dt.Date,
                Value = dt.SuccessRate,
                Label = dt.Date.ToString("dd/MM")
            }).ToList();
        }

        public async Task<List<ChartDataPoint>> GetExecutionVolumeTrendAsync(int days)
        {
            var dailyTrends = await GetDailyTrendsAsync(days);
            
            return dailyTrends.Select(dt => new ChartDataPoint
            {
                Date = dt.Date,
                Value = dt.TotalRuns,
                Label = dt.Date.ToString("dd/MM")
            }).ToList();
        }
    }
}