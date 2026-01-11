using System.ComponentModel.DataAnnotations;

namespace TesterLab.JobScheduler.Dtos
{
    public class CreateJobRequest
    {
       /* [Required]
        [MaxLength(200)]
        public string Name { get; set; }=string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
        */
        [Required]
        public DateTime FirstExecutionTimeUtc { get; set; }

        public int? FrequencyInMinutes { get; set; }

        public bool IsEnabled { get; set; } =   true;

        [Required]
        public int TestCaseId { get; set; }
    }

    public class UpdateJobRequest
    {
        /* [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
        */
        [Required]
        public DateTime NextExecutionTimeUtc { get; set; }

        public int? FrequencyInMinutes { get; set; }

        public bool IsEnabled { get; set; } = true;

        [Required]
        public int TestCaseId { get; set; }
    }

}
